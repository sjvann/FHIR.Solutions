using Fhir.Terminology.Core;
using Microsoft.AspNetCore.Http;

namespace Fhir.Terminology.App.Hosting;

/// <summary>將 <see cref="FhirOperationResult"/> 轉成 Minimal API 的 <see cref="IResult"/>（避免端點註冊重複條件式）。</summary>
internal static class FhirOperationResultHttpMapper
{
    public static IResult ToHttpResult(this FhirOperationResult result)
    {
        if (result.HttpStatus == 204)
            return Results.NoContent();
        return TypedResults.Content(result.JsonBody, result.ContentType, statusCode: result.HttpStatus);
    }
}
