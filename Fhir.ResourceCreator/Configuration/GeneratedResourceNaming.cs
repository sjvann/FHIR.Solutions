namespace FhirResourceCreator.Configuration;

/// <summary>
/// Naming for emitted resource libraries: one independent project per Registry package version.
/// Avoid suffix <c>Core</c> — in .NET that reads as a shared foundation; the only cross-FHIR-version shared library in this repo is <c>Fhir.TypeFramework</c>.
/// Version-scoped names like <c>Fhir.Resources.R5</c> signal “resource definitions for this FHIR line”, not “the shared core of all generators”.
/// </summary>
public static class GeneratedResourceNaming
{
    public const string LegacySentinelGlobalNamespace = "Fhir.Resources.Generated";

    /// <summary>
    /// Folder name, assembly name, NuGet id (unless overridden): e.g. <c>Fhir.Resources.R5</c>.
    /// </summary>
    public static string ResolveOutputProjectName(PackageGenerationTarget pkg, GeneratorOptions globals)
    {
        if (!string.IsNullOrWhiteSpace(pkg.OutputProjectName))
            return pkg.OutputProjectName.Trim();
        return SuggestOutputProjectName(pkg.PackageId);
    }

    /// <summary>
    /// POCO root namespace; defaults to <paramref name="outputProjectName"/> when unset.
    /// </summary>
    public static string ResolveRootNamespace(PackageGenerationTarget pkg, GeneratorOptions globals, string outputProjectName)
    {
        if (!string.IsNullOrWhiteSpace(pkg.RootNamespace))
            return pkg.RootNamespace.Trim();

        var g = globals.RootNamespace?.Trim();
        if (!string.IsNullOrEmpty(g) &&
            !string.Equals(g, LegacySentinelGlobalNamespace, StringComparison.Ordinal))
            return g;

        return outputProjectName;
    }

    /// <summary>
    /// Infer <c>Fhir.Resources.R5</c> / <c>Fhir.Resources.R4</c> from common HL7 package ids (e.g. <c>hl7.fhir.r5.core</c>).
    /// </summary>
    public static string SuggestOutputProjectName(string packageId)
    {
        if (string.IsNullOrWhiteSpace(packageId))
            return "Fhir.Resources.Unknown";

        var segments = packageId.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var segment in segments)
        {
            var p = segment;
            if (p.Length < 2 || p[0] != 'r' && p[0] != 'R')
                continue;
            if (!char.IsLetterOrDigit(p[1]))
                continue;
            var label = char.ToUpperInvariant(p[0]) + p.Substring(1);
            return $"Fhir.Resources.{label}";
        }

        var safe = new string(packageId.Where(static c => char.IsLetterOrDigit(c) || c is '_' or '-').ToArray());
        safe = safe.Replace('-', '_');
        return string.IsNullOrEmpty(safe) ? "Fhir.Resources.Unknown" : $"Fhir.Resources.{safe}";
    }
}
