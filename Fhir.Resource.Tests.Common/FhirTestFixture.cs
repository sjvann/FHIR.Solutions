using System.Reflection;

namespace Fhir.Resource.Tests.Common;

/// <summary>
/// Load JSON/XML fixture text for tests (filesystem or embedded resources).
/// </summary>
public static class FhirTestFixture
{
    /// <summary>Read entire file as UTF-8.</summary>
    public static string ReadUtf8File(string absolutePath)
    {
        if (!File.Exists(absolutePath))
            throw new FileNotFoundException("Fixture file not found.", absolutePath);
        return File.ReadAllText(absolutePath);
    }

    /// <summary>
    /// Load an embedded resource from <paramref name="assembly"/> (e.g. <c>TestData.Patient.min.json</c> with root namespace).
    /// </summary>
    public static string ReadEmbeddedResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource not found: {resourceName}. Ensure Build Action is EmbeddedResource.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
