using System.Text.Json;

namespace FhirResourceCreator.StructureDefinition;

public static class StructureDefinitionIO
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static StructureDefinitionDocument? Read(string path)
    {
        using var stream = File.OpenRead(path);
        return JsonSerializer.Deserialize<StructureDefinitionDocument>(stream, Options);
    }
}
