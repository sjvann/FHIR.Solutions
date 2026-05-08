namespace FhirResourceCreator.Configuration;

public enum GeneratorInputMode
{
    Registry,
    Excel
}

public sealed class GeneratorOptions
{
    public const string SectionName = "Generator";

    public GeneratorInputMode Mode { get; set; } = GeneratorInputMode.Registry;

    /// <summary>Primary FHIR NPM registry (trailing slash optional).</summary>
    public string RegistryBaseUrl { get; set; } = "https://packages.fhir.org";

    public string? RegistryFallbackUrl { get; set; } = "https://packages2.fhir.org";

    /// <summary>Extracted packages cache directory.</summary>
    public string PackageCacheDirectory { get; set; } = "artifacts/fhir-packages";

    /// <summary>Where generated solution folders are written.</summary>
    public string OutputRoot { get; set; } = "generated";

    /// <summary>Legacy Excel pipeline: folder containing *.xlsx definitions.</summary>
    public string? ExcelDefinitionsPath { get; set; }

    /// <summary>
    /// Optional global namespace root. Leave empty or use <see cref="GeneratedResourceNaming.LegacySentinelGlobalNamespace"/> so each package defaults to its emitted assembly name (e.g. <c>Fhir.Resources.R5</c>). Do not use suffix <c>Core</c> for generated resource assemblies — that implies a shared foundation; cross-version shared code lives in <c>Fhir.TypeFramework</c> only.
    /// </summary>
    public string RootNamespace { get; set; } = "";

    /// <summary>NuGet version string for Fhir.TypeFramework PackageReference in emitted projects.</summary>
    public string TypeFrameworkPackageVersion { get; set; } = "1.0.0";

    public List<PackageGenerationTarget> Packages { get; set; } = new();
}

public sealed class PackageGenerationTarget
{
    public string PackageId { get; set; } = "";

    public string Version { get; set; } = "";

    /// <summary>Emit folder / assembly / NuGet id (e.g. <c>Fhir.Resources.R5</c>). Empty: inferred from <see cref="PackageId"/>.</summary>
    public string OutputProjectName { get; set; } = "";

    /// <summary>Optional namespace override; defaults to assembly name when global root is unset.</summary>
    public string? RootNamespace { get; set; }

    /// <summary>If non-empty, only these resource type names (e.g. Patient) are generated.</summary>
    public List<string> ResourcesInclude { get; set; } = new();

    /// <summary>Resources to skip even if included by wildcard logic.</summary>
    public List<string> ResourcesExclude { get; set; } = new();
}
