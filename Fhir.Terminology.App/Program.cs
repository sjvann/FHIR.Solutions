using System.Text.Json;
using Fhir.Terminology.App;
using Fhir.Terminology.App.Components;
using Fhir.Terminology.App.OpenApi;
using Fhir.Terminology.Core;
using Fhir.Terminology.Infrastructure;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var terminologyConn = builder.Configuration.GetConnectionString("Terminology") ?? "Data Source=terminology.db";

builder.Services.AddOptions<TerminologyServerOptions>()
    .Configure(o => o.ConnectionString = terminologyConn)
    .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "SQLite connection string must not be empty")
    .ValidateOnStart();

builder.Services.AddDbContext<TerminologyDbContext>(o => o.UseSqlite(terminologyConn));
builder.Services.AddScoped<ITerminologyRepository, EfTerminologyRepository>();
builder.Services.AddHttpClient(RemoteTerminologyGateway.ClientName, client =>
{
    client.Timeout = TimeSpan.FromSeconds(120);
});
builder.Services.AddScoped<IRemoteTerminologyGateway, RemoteTerminologyGateway>();
builder.Services.AddScoped<TerminologyOrchestrator>();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "FHIR Terminology Service REST API",
            Version = "v1",
            Description = """
FHIR **R5** 術語服務：SQLite 儲存 **CodeSystem／ValueSet／ConceptMap**，支援 SHALL 術語操作與選配 **upstream** 委派。

**本機使用手冊（建議先讀）**：[應用程式內說明](/docs) 或首頁連結（同站 **Blazor**）。
儲存庫完整文件：`Fhir.Docs/terminology/user-manual.md`。

**重要**：
- 整合 FHIR 客戶端時 **Accept** 建議帶 `application/fhir+json`。
- 巨型詞彙（SNOMED／LOINC 等）通常需設定 **upstream**，不宜假設本機含完整詞表。
""",
        };
        document.Tags = new HashSet<OpenApiTag>
        {
            new() { Name = "系統", Description = "存活檢查（非 FHIR）" },
            new() { Name = "能力宣告", Description = "CapabilityStatement／TerminologyCapabilities" },
            new() { Name = "管理：Binding 登錄", Description = "Profile／元素與 ValueSet canonical（非標準 FHIR 資源路徑）" },
            new() { Name = "管理：Upstream", Description = "遠端術語伺服器路由設定" },
            new() { Name = "CodeSystem 術語操作", Description = "$lookup、$validate-code、$subsumes" },
            new() { Name = "ValueSet 術語操作", Description = "$expand、$validate-code" },
            new() { Name = "ConceptMap 術語操作", Description = "$translate" },
            new() { Name = "FHIR 資源 REST", Description = "CodeSystem／ValueSet／ConceptMap 之 SEARCH／READ／CREATE／UPDATE／DELETE" },
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

var seedDemoResources = app.Configuration.GetValue("Terminology:SeedDemoResources", true);
var seedExternalCanonical = app.Configuration.GetValue("Terminology:SeedExternalCanonicalCodeSystems", true);
var seedInternalFhir = app.Configuration.GetValue("Terminology:SeedInternalFhirCodeSystems", true);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TerminologyDbContext>();
    await db.Database.MigrateAsync();
    var repo = scope.ServiceProvider.GetRequiredService<ITerminologyRepository>();
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    if (seedDemoResources)
    {
        var seedLogger = loggerFactory.CreateLogger("TerminologyDemoSeed");
        await TerminologyDemoSeed.EnsureDemoResourcesAsync(db, repo, seedLogger, default);
    }
    if (seedExternalCanonical)
    {
        var extLogger = loggerFactory.CreateLogger("ExternalCanonicalCodeSystemsSeed");
        await ExternalCanonicalCodeSystemsSeed.EnsureRegisteredAsync(db, repo, extLogger, default);
    }
    if (seedInternalFhir)
    {
        var intLogger = loggerFactory.CreateLogger("InternalFhirCodeSystemsSeed");
        await InternalFhirCodeSystemsSeed.EnsureRegisteredAsync(db, repo, intLogger, default);
    }
}

var enableApiReference = builder.Configuration.GetValue("Terminology:EnableApiReference", true);
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

async Task<IResult> WriteFhir(FhirOperationResult r)
{
    if (r.HttpStatus == 204)
        return Results.NoContent();
    return TypedResults.Content(r.JsonBody, r.ContentType, statusCode: r.HttpStatus);
}

var admin = fhir.MapGroup("/admin/registry");

admin.MapGet("/bindings", async (TerminologyOrchestrator o, HttpContext ctx) =>
        await WriteFhir(await o.ListBindingsAsync(ctx.RequestAborted)))
    .WithFhirApiDocs(TerminologyEndpointDocs.AdminBindingsGetSummary, TerminologyEndpointDocs.AdminBindingsGetDetail);

admin.MapPost("/bindings", async (TerminologyOrchestrator o, HttpContext ctx) =>
    {
        var dto = await JsonSerializer.DeserializeAsync<BindingPostDto>(ctx.Request.Body, AdminJson.Options, ctx.RequestAborted);
        if (dto is null || string.IsNullOrWhiteSpace(dto.ValueSetCanonical) || string.IsNullOrWhiteSpace(dto.Strength))
            return Results.BadRequest();
        var row = new BindingRegistryRow(Guid.Empty, dto.StructureDefinitionUrl, dto.ElementPath, dto.ValueSetCanonical.Trim(), dto.Strength.Trim());
        return await WriteFhir(await o.AddBindingRowAsync(row, ctx.RequestAborted));
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.AdminBindingsPostSummary, TerminologyEndpointDocs.AdminBindingsPostDetail);

admin.MapDelete("/bindings/{id:guid}", async (TerminologyOrchestrator o, HttpContext ctx, Guid id) =>
        await WriteFhir(await o.DeleteBindingRowAsync(id, ctx.RequestAborted)))
    .WithFhirApiDocs(TerminologyEndpointDocs.AdminBindingsDeleteSummary, TerminologyEndpointDocs.AdminBindingsDeleteDetail);

admin.MapGet("/upstreams", async (TerminologyOrchestrator o, HttpContext ctx) =>
        await WriteFhir(await o.ListUpstreamsBundleAsync(ctx.RequestAborted)))
    .WithFhirApiDocs(TerminologyEndpointDocs.AdminUpstreamsGetSummary, TerminologyEndpointDocs.AdminUpstreamsGetDetail);

admin.MapPost("/upstreams", async (TerminologyOrchestrator o, HttpContext ctx) =>
    {
        var dto = await JsonSerializer.DeserializeAsync<UpstreamPostDto>(ctx.Request.Body, AdminJson.Options, ctx.RequestAborted);
        if (dto is null || string.IsNullOrWhiteSpace(dto.SystemUriPrefix) || string.IsNullOrWhiteSpace(dto.BaseUrl))
            return Results.BadRequest();
        var row = new UpstreamServerRow(Guid.Empty, dto.SystemUriPrefix.Trim(), dto.BaseUrl.Trim(), dto.AuthorizationHeader?.Trim(),
            dto.TimeoutSeconds <= 0 ? 60 : dto.TimeoutSeconds);
        return await WriteFhir(await o.AddUpstreamRowAsync(row, ctx.RequestAborted));
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.AdminUpstreamsPostSummary, TerminologyEndpointDocs.AdminUpstreamsPostDetail);

admin.MapDelete("/upstreams/{id:guid}", async (TerminologyOrchestrator o, HttpContext ctx, Guid id) =>
        await WriteFhir(await o.DeleteUpstreamRowAsync(id, ctx.RequestAborted)))
    .WithFhirApiDocs(TerminologyEndpointDocs.AdminUpstreamsDeleteSummary, TerminologyEndpointDocs.AdminUpstreamsDeleteDetail);

// Operations（須在泛型 /{id} 之前註冊，避免 $expand 等被當成 id）
fhir.MapGet("/CodeSystem/$lookup", async (TerminologyOrchestrator o, HttpContext ctx, string? code, string? system, string? version) =>
    {
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(system))
            return Results.BadRequest("code and system required");
        var r = await o.LookupAsync(null, system, code, version, ctx.RequestAborted);
        return await WriteFhir(r);
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.LookupSummary, TerminologyEndpointDocs.LookupDetail);

fhir.MapGet("/CodeSystem/$validate-code", async (TerminologyOrchestrator o, HttpContext ctx, string code, string? url, string? system) =>
    {
        var r = await o.ValidateCodeCodeSystemAsync(null, url, code, system, null, ctx.RequestAborted);
        return await WriteFhir(r);
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.ValidateCodeCsSummary, TerminologyEndpointDocs.ValidateCodeCsDetail);

fhir.MapGet("/CodeSystem/$subsumes", async (TerminologyOrchestrator o, HttpContext ctx, string codeA, string codeB, string system, string? version) =>
    {
        var r = await o.SubsumesAsync(null, system, codeA, codeB, version, ctx.RequestAborted);
        return await WriteFhir(r);
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.SubsumesSummary, TerminologyEndpointDocs.SubsumesDetail);

fhir.MapGet("/ValueSet/$expand", async (TerminologyOrchestrator o, HttpContext ctx, string? url, string? filter, int? offset, int? count) =>
    {
        var r = await o.ExpandValueSetAsync(null, url, filter, offset, count, ctx.RequestAborted);
        return await WriteFhir(r);
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.ExpandByUrlSummary, TerminologyEndpointDocs.ExpandByUrlDetail);

fhir.MapGet("/ValueSet/{id}/$expand", async (TerminologyOrchestrator o, HttpContext ctx, string id, string? filter, int? offset, int? count) =>
    {
        var r = await o.ExpandValueSetAsync(id, null, filter, offset, count, ctx.RequestAborted);
        return await WriteFhir(r);
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.ExpandByIdSummary, TerminologyEndpointDocs.ExpandByIdDetail);

fhir.MapGet("/ValueSet/$validate-code", async (TerminologyOrchestrator o, HttpContext ctx, string? url, string? profile, string? path, string? code, string? system, string? display) =>
    {
        if (!string.IsNullOrEmpty(profile) && !string.IsNullOrEmpty(path))
            return await WriteFhir(await o.ValidateCodeWithBindingContextAsync(profile, path, code, system, display, ctx.RequestAborted));

        var r = await o.ValidateCodeValueSetAsync(null, url, code, system, display, ctx.RequestAborted);
        return await WriteFhir(r);
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.ValidateCodeVsSummary, TerminologyEndpointDocs.ValidateCodeVsDetail);

fhir.MapGet("/ConceptMap/$translate", async (TerminologyOrchestrator o, HttpContext ctx, string code, string system, string? targetsystem, string? url) =>
    {
        var r = await o.TranslateAsync(null, url, code, system, targetsystem, ctx.RequestAborted);
        return await WriteFhir(r);
    })
    .WithFhirApiDocs(TerminologyEndpointDocs.TranslateSummary, TerminologyEndpointDocs.TranslateDetail);

foreach (var rt in new[] { "CodeSystem", "ValueSet", "ConceptMap" })
{
    var resourceType = rt;
    fhir.MapGet($"/{resourceType}", async (TerminologyOrchestrator o, HttpContext ctx, string? url, string? version, string? name, string? title, string? status) =>
        {
            var r = await o.SearchAsync(resourceType, new TerminologySearchParameters
            {
                Url = url,
                Version = version,
                Name = name,
                Title = title,
                Status = status,
            }, ctx.RequestAborted);
            return await WriteFhir(r);
        })
        .WithFhirApiDocs(TerminologyEndpointDocs.SearchSummary(resourceType), TerminologyEndpointDocs.SearchDetail(resourceType));

    fhir.MapGet($"/{resourceType}/{{id}}", async (TerminologyOrchestrator o, HttpContext ctx, string id) =>
        {
            var r = await o.ReadAsync(resourceType, id, ctx.RequestAborted);
            return r is null ? Results.NotFound() : await WriteFhir(r);
        })
        .WithFhirApiDocs(TerminologyEndpointDocs.ReadSummary(resourceType), TerminologyEndpointDocs.ReadDetail(resourceType));

    fhir.MapPost($"/{resourceType}", async (TerminologyOrchestrator o, HttpContext ctx) =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            var json = await reader.ReadToEndAsync(ctx.RequestAborted);
            var r = await o.CreateAsync(resourceType, json, ctx.RequestAborted);
            return await WriteFhir(r);
        })
        .WithFhirApiDocs(TerminologyEndpointDocs.CreateSummary(resourceType), TerminologyEndpointDocs.CreateDetail(resourceType));

    fhir.MapPut($"/{resourceType}/{{id}}", async (TerminologyOrchestrator o, HttpContext ctx, string id) =>
        {
            using var reader = new StreamReader(ctx.Request.Body);
            var json = await reader.ReadToEndAsync(ctx.RequestAborted);
            var r = await o.UpdateAsync(resourceType, id, json, ctx.RequestAborted);
            return await WriteFhir(r);
        })
        .WithFhirApiDocs(TerminologyEndpointDocs.UpdateSummary(resourceType), TerminologyEndpointDocs.UpdateDetail(resourceType));

    fhir.MapMethods($"/{resourceType}/{{id}}", ["DELETE"], async (TerminologyOrchestrator o, HttpContext ctx, string id) =>
        {
            var r = await o.DeleteAsync(resourceType, id, ctx.RequestAborted);
            return await WriteFhir(r);
        })
        .WithFhirApiDocs(TerminologyEndpointDocs.DeleteSummary(resourceType), TerminologyEndpointDocs.DeleteDetail(resourceType));
}

app.MapStaticAssets();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

public partial class Program { }