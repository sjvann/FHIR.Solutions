using System.Text.Json;
using Fhir.Terminology.App.OpenApi;
using Fhir.Terminology.Core;
using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;

namespace Fhir.Terminology.App.Hosting;

internal static class TerminologyWebApplicationExtensions
{
    /// <summary>若設定啟用，註冊 OpenAPI JSON 與 Scalar UI。</summary>
    public static WebApplication UseTerminologyOpenApiReference(this WebApplication app)
    {
        var enableApiReference = app.Configuration.GetValue("Terminology:EnableApiReference", true);
        if (enableApiReference)
        {
            app.MapOpenApi("/openapi/{documentName}.json");
            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("FHIR Terminology Service REST API")
                    .WithOpenApiRoutePattern("/openapi/{documentName}.json");
            });
        }

        return app;
    }

    /// <summary>註冊 Antiforgery、FHIR／管理 REST、靜態資源與 Blazor。</summary>
    public static WebApplication MapTerminologyApplicationPipeline(this WebApplication app)
    {
        app.UseAntiforgery();

        app.MapGet("/health", () => Results.Ok("ok"))
            .WithFhirApiDocs(TerminologyEndpointDocs.HealthSummary, TerminologyEndpointDocs.HealthDetail);

        var fhir = app.MapGroup("/fhir");

        fhir.MapGet("/metadata", async (TerminologyOrchestrator o, HttpContext ctx, string? mode) =>
        {
            var r = string.Equals(mode, "terminology", StringComparison.OrdinalIgnoreCase)
                ? await o.MetadataTerminologyAsync(ctx.RequestAborted)
                : await o.MetadataCapabilityAsync(ctx.RequestAborted);
            return TypedResults.Content(r.JsonBody, r.ContentType, statusCode: r.HttpStatus);
        }).WithFhirApiDocs(TerminologyEndpointDocs.MetadataSummary, TerminologyEndpointDocs.MetadataDetail);

        MapAdminRegistryRoutes(fhir);
        MapTerminologyOperationRoutes(fhir);
        MapFhirResourceTypeRoutes(fhir);

        app.MapStaticAssets();

        app.MapRazorComponents<Fhir.Terminology.App.Components.App>().AddInteractiveServerRenderMode();

        return app;
    }

    private static void MapAdminRegistryRoutes(RouteGroupBuilder fhir)
    {
        var admin = fhir.MapGroup("/admin/registry");

        admin.MapGet("/bindings", async (TerminologyOrchestrator o, HttpContext ctx) =>
                (await o.ListBindingsAsync(ctx.RequestAborted)).ToHttpResult())
            .WithFhirApiDocs(TerminologyEndpointDocs.AdminBindingsGetSummary, TerminologyEndpointDocs.AdminBindingsGetDetail);

        admin.MapPost("/bindings", async (TerminologyOrchestrator o, HttpContext ctx) =>
            {
                var dto = await JsonSerializer.DeserializeAsync<BindingPostDto>(ctx.Request.Body, AdminJson.Options, ctx.RequestAborted);
                if (dto is null || string.IsNullOrWhiteSpace(dto.ValueSetCanonical) || string.IsNullOrWhiteSpace(dto.Strength))
                    return Results.BadRequest();
                var row = new BindingRegistryRow(Guid.Empty, dto.StructureDefinitionUrl, dto.ElementPath, dto.ValueSetCanonical.Trim(), dto.Strength.Trim());
                return (await o.AddBindingRowAsync(row, ctx.RequestAborted)).ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.AdminBindingsPostSummary, TerminologyEndpointDocs.AdminBindingsPostDetail);

        admin.MapDelete("/bindings/{id:guid}", async (TerminologyOrchestrator o, HttpContext ctx, Guid id) =>
                (await o.DeleteBindingRowAsync(id, ctx.RequestAborted)).ToHttpResult())
            .WithFhirApiDocs(TerminologyEndpointDocs.AdminBindingsDeleteSummary, TerminologyEndpointDocs.AdminBindingsDeleteDetail);

        admin.MapGet("/upstreams", async (TerminologyOrchestrator o, HttpContext ctx) =>
                (await o.ListUpstreamsBundleAsync(ctx.RequestAborted)).ToHttpResult())
            .WithFhirApiDocs(TerminologyEndpointDocs.AdminUpstreamsGetSummary, TerminologyEndpointDocs.AdminUpstreamsGetDetail);

        admin.MapPost("/upstreams", async (TerminologyOrchestrator o, HttpContext ctx) =>
            {
                var dto = await JsonSerializer.DeserializeAsync<UpstreamPostDto>(ctx.Request.Body, AdminJson.Options, ctx.RequestAborted);
                if (dto is null || string.IsNullOrWhiteSpace(dto.SystemUriPrefix) || string.IsNullOrWhiteSpace(dto.BaseUrl))
                    return Results.BadRequest();
                var row = new UpstreamServerRow(Guid.Empty, dto.SystemUriPrefix.Trim(), dto.BaseUrl.Trim(), dto.AuthorizationHeader?.Trim(),
                    dto.TimeoutSeconds <= 0 ? 60 : dto.TimeoutSeconds);
                return (await o.AddUpstreamRowAsync(row, ctx.RequestAborted)).ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.AdminUpstreamsPostSummary, TerminologyEndpointDocs.AdminUpstreamsPostDetail);

        admin.MapDelete("/upstreams/{id:guid}", async (TerminologyOrchestrator o, HttpContext ctx, Guid id) =>
                (await o.DeleteUpstreamRowAsync(id, ctx.RequestAborted)).ToHttpResult())
            .WithFhirApiDocs(TerminologyEndpointDocs.AdminUpstreamsDeleteSummary, TerminologyEndpointDocs.AdminUpstreamsDeleteDetail);
    }

    private static void MapTerminologyOperationRoutes(RouteGroupBuilder fhir)
    {
        fhir.MapGet("/CodeSystem/$lookup", async (TerminologyOrchestrator o, HttpContext ctx, string? code, string? system, string? version, string? fhirSpecVersion) =>
            {
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(system))
                    return Results.BadRequest("code and system required");
                var r = await o.LookupAsync(null, system, code, version, fhirSpecVersion, ctx.RequestAborted);
                return r.ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.LookupSummary, TerminologyEndpointDocs.LookupDetail);

        fhir.MapGet("/CodeSystem/$validate-code", async (TerminologyOrchestrator o, HttpContext ctx, string code, string? url, string? system, string? fhirSpecVersion) =>
            {
                var r = await o.ValidateCodeCodeSystemAsync(null, url, code, system, null, fhirSpecVersion, ctx.RequestAborted);
                return r.ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.ValidateCodeCsSummary, TerminologyEndpointDocs.ValidateCodeCsDetail);

        fhir.MapGet("/CodeSystem/$subsumes", async (TerminologyOrchestrator o, HttpContext ctx, string codeA, string codeB, string system, string? version, string? fhirSpecVersion) =>
            {
                var r = await o.SubsumesAsync(null, system, codeA, codeB, version, fhirSpecVersion, ctx.RequestAborted);
                return r.ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.SubsumesSummary, TerminologyEndpointDocs.SubsumesDetail);

        fhir.MapGet("/ValueSet/$expand", async (TerminologyOrchestrator o, HttpContext ctx, string? url, string? filter, int? offset, int? count, string? fhirSpecVersion) =>
            {
                var r = await o.ExpandValueSetAsync(null, url, filter, offset, count, fhirSpecVersion, ctx.RequestAborted);
                return r.ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.ExpandByUrlSummary, TerminologyEndpointDocs.ExpandByUrlDetail);

        fhir.MapGet("/ValueSet/{id}/$expand", async (TerminologyOrchestrator o, HttpContext ctx, string id, string? filter, int? offset, int? count, string? fhirSpecVersion) =>
            {
                var r = await o.ExpandValueSetAsync(id, null, filter, offset, count, fhirSpecVersion, ctx.RequestAborted);
                return r.ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.ExpandByIdSummary, TerminologyEndpointDocs.ExpandByIdDetail);

        fhir.MapGet("/ValueSet/$validate-code", async (TerminologyOrchestrator o, HttpContext ctx, string? url, string? profile, string? path, string? code, string? system, string? display, string? fhirSpecVersion) =>
            {
                if (!string.IsNullOrEmpty(profile) && !string.IsNullOrEmpty(path))
                    return (await o.ValidateCodeWithBindingContextAsync(profile, path, code, system, display, ctx.RequestAborted)).ToHttpResult();

                var r = await o.ValidateCodeValueSetAsync(null, url, code, system, display, fhirSpecVersion, ctx.RequestAborted);
                return r.ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.ValidateCodeVsSummary, TerminologyEndpointDocs.ValidateCodeVsDetail);

        fhir.MapGet("/ConceptMap/$translate", async (TerminologyOrchestrator o, HttpContext ctx, string code, string system, string? targetsystem, string? url, string? fhirSpecVersion) =>
            {
                var r = await o.TranslateAsync(null, url, code, system, targetsystem, fhirSpecVersion, ctx.RequestAborted);
                return r.ToHttpResult();
            })
            .WithFhirApiDocs(TerminologyEndpointDocs.TranslateSummary, TerminologyEndpointDocs.TranslateDetail);
    }

    private static void MapFhirResourceTypeRoutes(RouteGroupBuilder fhir)
    {
        foreach (var rt in new[] { "CodeSystem", "ValueSet", "ConceptMap", "StructureDefinition" })
        {
            var resourceType = rt;
            fhir.MapGet($"/{resourceType}", async (TerminologyOrchestrator o, HttpContext ctx, string? url, string? version, string? name, string? title, string? status, string? fhirSpecVersion) =>
                {
                    var r = await o.SearchAsync(resourceType, new TerminologySearchParameters
                    {
                        Url = url,
                        Version = version,
                        Name = name,
                        Title = title,
                        Status = status,
                    }, fhirSpecVersion, ctx.RequestAborted);
                    return r.ToHttpResult();
                })
                .WithFhirApiDocs(TerminologyEndpointDocs.SearchSummary(resourceType), TerminologyEndpointDocs.SearchDetail(resourceType));

            fhir.MapGet($"/{resourceType}/{{id}}", async (TerminologyOrchestrator o, HttpContext ctx, string id, string? fhirSpecVersion) =>
                {
                    var r = await o.ReadAsync(resourceType, id, fhirSpecVersion, ctx.RequestAborted);
                    return r is null ? Results.NotFound() : r.ToHttpResult();
                })
                .WithFhirApiDocs(TerminologyEndpointDocs.ReadSummary(resourceType), TerminologyEndpointDocs.ReadDetail(resourceType));

            fhir.MapPost($"/{resourceType}", async (TerminologyOrchestrator o, HttpContext ctx) =>
                {
                    using var reader = new StreamReader(ctx.Request.Body);
                    var json = await reader.ReadToEndAsync(ctx.RequestAborted);
                    var r = await o.CreateAsync(resourceType, json, ctx.RequestAborted);
                    return r.ToHttpResult();
                })
                .WithFhirApiDocs(TerminologyEndpointDocs.CreateSummary(resourceType), TerminologyEndpointDocs.CreateDetail(resourceType));

            fhir.MapPut($"/{resourceType}/{{id}}", async (TerminologyOrchestrator o, HttpContext ctx, string id, string? fhirSpecVersion) =>
                {
                    using var reader = new StreamReader(ctx.Request.Body);
                    var json = await reader.ReadToEndAsync(ctx.RequestAborted);
                    var r = await o.UpdateAsync(resourceType, id, json, fhirSpecVersion, ctx.RequestAborted);
                    return r.ToHttpResult();
                })
                .WithFhirApiDocs(TerminologyEndpointDocs.UpdateSummary(resourceType), TerminologyEndpointDocs.UpdateDetail(resourceType));

            fhir.MapMethods($"/{resourceType}/{{id}}", ["DELETE"], async (TerminologyOrchestrator o, HttpContext ctx, string id, string? fhirSpecVersion) =>
                {
                    var r = await o.DeleteAsync(resourceType, id, fhirSpecVersion, ctx.RequestAborted);
                    return r.ToHttpResult();
                })
                .WithFhirApiDocs(TerminologyEndpointDocs.DeleteSummary(resourceType), TerminologyEndpointDocs.DeleteDetail(resourceType));
        }
    }
}
