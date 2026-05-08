using System.Text.Json;

namespace Fhir.Terminology.App;

internal static class AdminJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}

internal sealed record BindingPostDto(string? StructureDefinitionUrl, string? ElementPath, string ValueSetCanonical, string Strength);

internal sealed record UpstreamPostDto(string SystemUriPrefix, string BaseUrl, string? AuthorizationHeader, int TimeoutSeconds);
