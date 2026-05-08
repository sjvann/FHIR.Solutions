using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Fhir.Terminology.App.OpenApi;

/// <summary>為 Minimal API 加上 Scalar／OpenAPI 的 <see cref="OpenApiOperation.Summary"/> 與 <see cref="OpenApiOperation.Description"/>（支援 CommonMark）。</summary>
internal static class FhirOpenApiExtensions
{
    public static TBuilder WithFhirApiDocs<TBuilder>(this TBuilder builder, string summary, string? description = null)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.AddOpenApiOperationTransformer((operation, _, _) =>
        {
            operation.Summary = summary;
            operation.Description = description;
            return Task.CompletedTask;
        });
    }
}
