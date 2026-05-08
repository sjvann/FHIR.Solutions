using System.Text;
using FhirResourceCreator.Configuration;
using FhirResourceCreator.Generation;
using FhirResourceCreator.Models;
using FhirResourceCreator.Registry;
using FhirResourceCreator.StructureDefinition;

namespace FhirResourceCreator.Pipeline;

public static class GenerationOrchestrator
{
    public static async Task RunRegistryPackagesAsync(GeneratorOptions options, CancellationToken ct = default)
    {
        using var http = FhirPackageDownloader.CreateDefaultClient();
        var downloader = new FhirPackageDownloader(http);

        foreach (var pkg in options.Packages)
        {
            if (string.IsNullOrWhiteSpace(pkg.PackageId) || string.IsNullOrWhiteSpace(pkg.Version))
                continue;

            var cacheRoot = Path.Combine(options.PackageCacheDirectory, SanitizePath(pkg.PackageId), pkg.Version);
            Directory.CreateDirectory(cacheRoot);
            var packageDir = Path.Combine(cacheRoot, "package");
            if (!Directory.Exists(packageDir))
            {
                string tgzPath;
                try
                {
                    tgzPath = await downloader.DownloadPackageTarballAsync(
                        options.RegistryBaseUrl, pkg.PackageId, pkg.Version, cacheRoot, ct).ConfigureAwait(false);
                }
                catch
                {
                    if (!string.IsNullOrEmpty(options.RegistryFallbackUrl))
                    {
                        tgzPath = await downloader.DownloadPackageTarballAsync(
                            options.RegistryFallbackUrl!, pkg.PackageId, pkg.Version, cacheRoot, ct).ConfigureAwait(false);
                    }
                    else
                        throw;
                }

                NpmPackageExtractor.ExtractTarGz(tgzPath, cacheRoot);
            }

            if (!Directory.Exists(packageDir))
                throw new InvalidOperationException($"Extracted package folder not found: {packageDir}");

            var exampleIndex = FhirPackageExampleHarvester.BuildResourceInstanceIndex(packageDir);

            var jsonFiles = Directory.GetFiles(packageDir, "*.json", SearchOption.AllDirectories);
            var emittedTypes = new HashSet<string>(StringComparer.Ordinal);
            var outputName = GeneratedResourceNaming.ResolveOutputProjectName(pkg, options);
            var rootNs = GeneratedResourceNaming.ResolveRootNamespace(pkg, options, outputName);
            var projDir = Path.Combine(options.OutputRoot, outputName);
            Directory.CreateDirectory(projDir);

            foreach (var file in jsonFiles)
            {
                ct.ThrowIfCancellationRequested();
                StructureDefinitionDocument? doc;
                try
                {
                    doc = StructureDefinitionIO.Read(file);
                }
                catch
                {
                    continue;
                }

                if (doc is null)
                    continue;
                if (!string.Equals(doc.ResourceType, "StructureDefinition", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (!string.Equals(doc.Kind, "resource", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (doc.Abstract == true)
                    continue;
                if (doc.Snapshot?.Element is null || doc.Snapshot.Element.Count == 0)
                    continue;

                var typeName = doc.Type ?? doc.Id ?? doc.Name;
                if (string.IsNullOrEmpty(typeName))
                    continue;

                if (pkg.ResourcesInclude.Count > 0 &&
                    !pkg.ResourcesInclude.Contains(typeName, StringComparer.OrdinalIgnoreCase))
                    continue;
                if (pkg.ResourcesExclude.Contains(typeName, StringComparer.OrdinalIgnoreCase))
                    continue;

                if (!emittedTypes.Add(typeName))
                    continue;

                var records = SnapshotToElementRecords.Convert(doc, typeName);
                if (records.Count == 0)
                    continue;

                var model = new ResourceModel(typeName, projDir, rootNs, records);
                var code = PocoResourceGenerator.Generate(model, rootNs);
                var outCs = Path.Combine(projDir, $"{typeName}.cs");
                await File.WriteAllTextAsync(outCs, code, ct).ConfigureAwait(false);

                var testDir = Path.Combine(projDir, $"{outputName}.Tests");
                Directory.CreateDirectory(testDir);
                var testDataDir = Path.Combine(testDir, "TestData");
                var fixtureFiles = FhirPackageExampleHarvester.CopyExamplesToTestData(typeName, exampleIndex, testDataDir);
                EmitResourceTests(projDir, rootNs, typeName, outputName, records, fixtureFiles);
            }

            var tfCsproj = Path.Combine(FindRepoRoot(), "Fhir.TypeFramework", "Fhir.TypeFramework.csproj");
            var tfRelLib = File.Exists(tfCsproj) ? Path.GetRelativePath(projDir, tfCsproj) : null;

            ProjectEmitter.WriteLibraryProject(
                Path.Combine(projDir, $"{outputName}.csproj"),
                pkg,
                options,
                tfRelLib,
                emitAssemblyName: outputName,
                emitRootNamespace: rootNs);

            var testProjDir = Path.Combine(projDir, $"{outputName}.Tests");
            if (Directory.Exists(testProjDir))
            {
                var libRel = $"..\\{outputName}.csproj";
                var commonRel = Path.GetRelativePath(testProjDir, Path.Combine(FindRepoRoot(), "Fhir.Resource.Tests.Common", "Fhir.Resource.Tests.Common.csproj"));
                var tfRelTest = File.Exists(tfCsproj) ? Path.GetRelativePath(testProjDir, tfCsproj) : null;
                ProjectEmitter.WriteTestProject(
                    Path.Combine(testProjDir, $"{outputName}.Tests.csproj"),
                    pkg,
                    options,
                    libraryProjectRelativePath: libRel,
                    testsCommonRelativePath: commonRel,
                    typeFrameworkProjectRelativePathFromTestDir: tfRelTest,
                    emitAssemblyName: outputName,
                    emitRootNamespace: rootNs);
            }
        }
    }

    static string FindRepoRoot()
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var dir = start;
            for (var i = 0; i < 12 && !string.IsNullOrEmpty(dir); i++)
            {
                if (Directory.GetFiles(dir, "*.slnx").Length > 0 || Directory.GetFiles(dir, "*.sln").Length > 0)
                    return dir;
                dir = Directory.GetParent(dir)?.FullName ?? "";
            }
        }
        return Directory.GetCurrentDirectory();
    }

    static void EmitResourceTests(
        string projDir,
        string rootNs,
        string typeName,
        string outputProjectName,
        IReadOnlyList<ElementRecord> records,
        IReadOnlyList<string> fixtureRelativePaths)
    {
        var testDir = Path.Combine(projDir, $"{outputProjectName}.Tests");
        Directory.CreateDirectory(testDir);
        var testNs = rootNs + ".Tests";
        var initBody = ResourceTestConstructionEmitter.EmitObjectInitializerBody(typeName, records);

        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("// Layer-2: fixture JSON from hl7 FHIR NPM package (plus fallback minimal) + constructed instance.");
        sb.AppendLine("// JSON is written via ITestOutputHelper (Test Explorer output pane).");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using System.IO;");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine("using Xunit;");
        sb.AppendLine("using Xunit.Abstractions;");
        sb.AppendLine("using Fhir.Resource.Tests.Common;");
        sb.AppendLine("using Fhir.TypeFramework.DataTypes;");
        sb.AppendLine("using Fhir.TypeFramework.DataTypes.PrimitiveTypes;");
        sb.AppendLine($"using {rootNs};");
        sb.AppendLine();
        sb.AppendLine($"namespace {testNs};");
        sb.AppendLine();
        sb.AppendLine($"public class {typeName}SerializationTests");
        sb.AppendLine("{");
        sb.AppendLine("    private readonly ITestOutputHelper _output;");
        sb.AppendLine();
        sb.AppendLine($"    public {typeName}SerializationTests(ITestOutputHelper output) => _output = output;");
        sb.AppendLine();
        sb.AppendLine("    public static IEnumerable<object[]> OfficialExampleRelativePaths =>");
        sb.AppendLine("        new object[][]");
        sb.AppendLine("        {");
        foreach (var rel in fixtureRelativePaths)
            sb.AppendLine($"            new object[] {{ \"{rel}\" }},");
        sb.AppendLine("        };");
        sb.AppendLine();
        sb.AppendLine("    [Theory]");
        sb.AppendLine("    [MemberData(nameof(OfficialExampleRelativePaths))]");
        sb.AppendLine("    public void Deserialize_official_fixtures_then_roundtrip_and_assert_id(string relativePath)");
        sb.AppendLine("    {");
        sb.AppendLine("        var path = Path.Combine(AppContext.BaseDirectory, \"TestData\", relativePath);");
        sb.AppendLine("        Assert.True(File.Exists(path), \"Fixture missing: \" + path);");
        sb.AppendLine("        var json = File.ReadAllText(path);");
        sb.AppendLine("        _output.WriteLine(\"=== Fixture JSON ===\");");
        sb.AppendLine("        _output.WriteLine(json);");
        sb.AppendLine("        string? expectedId = null;");
        sb.AppendLine("        using (var jd = JsonDocument.Parse(json))");
        sb.AppendLine("        {");
        sb.AppendLine("            if (jd.RootElement.TryGetProperty(\"id\", out var idEl) && idEl.ValueKind == JsonValueKind.String)");
        sb.AppendLine("                expectedId = idEl.GetString();");
        sb.AppendLine("        }");
        sb.AppendLine($"        var r = FhirJsonRoundTrip.Deserialize<{typeName}>(json);");
        sb.AppendLine("        Assert.NotNull(r);");
        sb.AppendLine("        if (expectedId is not null)");
        sb.AppendLine("            Assert.Equal(expectedId, (string?)r!.Id);");
        sb.AppendLine("        var back = FhirJsonRoundTrip.Serialize(r!);");
        sb.AppendLine("        _output.WriteLine(\"=== Round-trip JSON ===\");");
        sb.AppendLine("        _output.WriteLine(back);");
        sb.AppendLine("        Assert.Contains(\"\\\"resourceType\\\"\", back);");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    [Fact]");
        sb.AppendLine("    public void Serialize_constructed_instance_including_required_primitives()");
        sb.AppendLine("    {");
        sb.AppendLine($"        var r = new {typeName}");
        sb.AppendLine("        {");
        sb.Append(initBody);
        sb.AppendLine("        };");
        sb.AppendLine("        var json = FhirJsonRoundTrip.Serialize(r);");
        sb.AppendLine("        _output.WriteLine(\"=== Constructed instance JSON ===\");");
        sb.AppendLine("        _output.WriteLine(json);");
        sb.AppendLine("        Assert.Contains(\"\\\"resourceType\\\"\", json);");
        sb.AppendLine($"        Assert.Contains(\"\\\"{typeName}\\\"\", json);");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        sb.AppendLine();
        File.WriteAllText(Path.Combine(testDir, $"{typeName}SerializationTests.cs"), sb.ToString());
    }

    static string SanitizePath(string packageId) => packageId.Replace('/', '-').Replace('\\', '-');

    public static void RunExcelLegacy(GeneratorOptions options)
    {
        var templateSec = options.ExcelDefinitionsPath
            ?? throw new InvalidOperationException("ExcelDefinitionsPath is required for Excel mode.");
        var saveTo = options.OutputRoot;
        var ns = string.IsNullOrWhiteSpace(options.RootNamespace)
            ? "Fhir.Resources.Excel"
            : options.RootNamespace;
        Directory.CreateDirectory(saveTo);
        RemoveGeneratedFiles(saveTo);

        var filter = options.Packages.FirstOrDefault()?.ResourcesInclude;
        foreach (var file in Directory.GetFiles(templateSec, "*.xlsx", SearchOption.TopDirectoryOnly))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            if (filter is { Count: > 0 } && !filter.Contains(name, StringComparer.OrdinalIgnoreCase))
                continue;

            var rm = new ResourceModel(file, saveTo, ns);
            if (rm.ResourceName is null)
                continue;
            var code = PocoResourceGenerator.Generate(rm, ns);
            var outPath = Path.Combine(saveTo, $"{rm.ResourceName}.generated.cs");
            File.WriteAllText(outPath, code);
        }
    }

    static void RemoveGeneratedFiles(string saveTo)
    {
        if (!Directory.Exists(saveTo))
            return;
        foreach (var f in Directory.GetFiles(saveTo, "*.generated.cs", SearchOption.TopDirectoryOnly))
            File.Delete(f);
    }
}
