using System.Text;
using FhirResourceCreator.Configuration;

namespace FhirResourceCreator.Generation;

public static class ProjectEmitter
{
    /// <param name="emitAssemblyName">Resolved folder/assembly/package id (see <see cref="GeneratedResourceNaming"/>).</param>
    /// <param name="emitRootNamespace">Resolved POCO root namespace.</param>
    /// <param name="typeFrameworkProjectRelativePath">If set, use ProjectReference to local TypeFramework (dev); otherwise NuGet package.</param>
    public static void WriteLibraryProject(
        string csprojPath,
        PackageGenerationTarget pkg,
        GeneratorOptions globals,
        string? typeFrameworkProjectRelativePath,
        string emitAssemblyName,
        string emitRootNamespace)
    {
        var tfVer = globals.TypeFrameworkPackageVersion;
        var asm = emitAssemblyName;
        var ns = emitRootNamespace;
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net10.0</TargetFramework>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine($"    <RootNamespace>{ns}</RootNamespace>");
        sb.AppendLine($"    <AssemblyName>{asm}</AssemblyName>");
        sb.AppendLine("    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>");
        sb.AppendLine($"    <PackageId>{asm}</PackageId>");
        sb.AppendLine($"    <PackageVersion>{pkg.Version}</PackageVersion>");
        sb.AppendLine($"    <Description>FHIR resource POCOs generated from {pkg.PackageId} ({pkg.Version}).</Description>");
        sb.AppendLine("    <PackageTags>FHIR;HL7;generated</PackageTags>");
        sb.AppendLine("    <PackageLicenseExpression>MIT</PackageLicenseExpression>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine("    <Compile Remove=\"**/*.Tests/**/*.cs\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("  <ItemGroup>");
        if (!string.IsNullOrEmpty(typeFrameworkProjectRelativePath))
            sb.AppendLine($"    <ProjectReference Include=\"{typeFrameworkProjectRelativePath}\" />");
        else
            sb.AppendLine($"    <PackageReference Include=\"Fhir.TypeFramework\" Version=\"{tfVer}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        File.WriteAllText(csprojPath, sb.ToString());
    }

    public static void WriteTestProject(
        string csprojPath,
        PackageGenerationTarget pkg,
        GeneratorOptions globals,
        string libraryProjectRelativePath,
        string testsCommonRelativePath,
        string? typeFrameworkProjectRelativePathFromTestDir,
        string emitAssemblyName,
        string emitRootNamespace)
    {
        var asm = emitAssemblyName + ".Tests";
        var ns = emitRootNamespace + ".Tests";
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net10.0</TargetFramework>");
        sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
        sb.AppendLine("    <Nullable>enable</Nullable>");
        sb.AppendLine("    <IsPackable>false</IsPackable>");
        sb.AppendLine($"    <RootNamespace>{ns}</RootNamespace>");
        sb.AppendLine($"    <AssemblyName>{asm}</AssemblyName>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine("    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.14.1\" />");
        sb.AppendLine("    <PackageReference Include=\"xunit\" Version=\"2.9.3\" />");
        sb.AppendLine("    <PackageReference Include=\"xunit.runner.visualstudio\" Version=\"3.1.4\">");
        sb.AppendLine("      <PrivateAssets>all</PrivateAssets>");
        sb.AppendLine("      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>");
        sb.AppendLine("    </PackageReference>");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine($"    <ProjectReference Include=\"{libraryProjectRelativePath}\" />");
        sb.AppendLine($"    <ProjectReference Include=\"{testsCommonRelativePath}\" />");
        if (!string.IsNullOrEmpty(typeFrameworkProjectRelativePathFromTestDir))
            sb.AppendLine($"    <ProjectReference Include=\"{typeFrameworkProjectRelativePathFromTestDir}\" />");
        else
            sb.AppendLine($"    <PackageReference Include=\"Fhir.TypeFramework\" Version=\"{globals.TypeFrameworkPackageVersion}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("  <ItemGroup>");
        sb.AppendLine("    <None Include=\"TestData\\**\\*\" CopyToOutputDirectory=\"PreserveNewest\" LinkBase=\"TestData\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        File.WriteAllText(csprojPath, sb.ToString());
    }
}
