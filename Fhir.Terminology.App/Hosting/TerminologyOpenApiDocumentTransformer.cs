using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Fhir.Terminology.App.Hosting;

/// <summary>集中設定術語服務 OpenAPI 文件資訊與標籤（與 HTTP 管線分離）。</summary>
internal static class TerminologyOpenApiDocumentTransformer
{
    internal static Task ConfigureDocumentAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
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
    }
}
