namespace Fhir.Terminology.App.OpenApi;

/// <summary>Scalar／OpenAPI 各端點之中文摘要與說明（CommonMark）。</summary>
internal static class TerminologyEndpointDocs
{
    public const string HealthSummary = "存活檢查（Liveness／Probe）";
    public const string HealthDetail = """
負載平衡器或 Kubernetes **liveness** 探針使用。
**回應**：HTTP 200，本文為純文字 `ok`（非 FHIR JSON）。
""";

    public const string MetadataSummary = "取得能力宣告（metadata）";
    public const string MetadataDetail = """
對應 FHIR **`GET [base]/metadata`**。

- **查詢參數 `mode`**（選填）：
  - 省略或無值 → 回傳 **`CapabilityStatement`**（`application/fhir+json`）。
  - `terminology` → 回傳 **`TerminologyCapabilities`**（本機已儲存之 CodeSystem 等）。
- **用途**：客戶端探索伺服器支援之資源類型與術語能力。
""";

    public const string AdminBindingsGetSummary = "列出 FHIR Binding 登錄（Bundle）";
    public const string AdminBindingsGetDetail = """
**回應**：`Bundle`（`type` 建議為 collection），每筆 `entry` 內 **`Parameters`** 含登錄欄位（如 `id`、`structureDefinition`、`path`、`valueSet`、`strength`）。

**用途**：維運檢視「Profile／元素 path」與 **ValueSet canonical**、**綁定強度** 之對照，供 `$validate-code?profile=&path=` 使用。
""";

    public const string AdminBindingsPostSummary = "新增一筆 Binding 登錄";
    public const string AdminBindingsPostDetail = """
**Content-Type**：`application/json`（**非** FHIR 資源，為本產品管理用 DTO）。

**本文欄位**（camelCase）：
- `valueSetCanonical`（必填）
- `strength`（必填，如 `required` / `extensible` / `preferred` / `example`）
- `structureDefinitionUrl`（選填）
- `elementPath`（選填，如 `Patient.gender`）

**成功**：HTTP 201，本文含新列 **id**（`Parameters`）。
""";

    public const string AdminBindingsDeleteSummary = "刪除指定 Binding 登錄";
    public const string AdminBindingsDeleteDetail = "路徑參數 **id** 為登錄列之 GUID。成功時 HTTP **204** 無本文。";

    public const string AdminUpstreamsGetSummary = "列出遠端術語上游（Bundle）";
    public const string AdminUpstreamsGetDetail = """
**回應**：`Bundle`，每筆為 **`Parameters`**，含 `id`、`systemUriPrefix`、`baseUrl`、`timeoutSeconds` 等。

當本機無資源或無法完成操作時，可能將同一 FHIR 操作 **HTTP 委派**至此處設定之上游（須能連線至對方 **`/fhir`**）。
""";

    public const string AdminUpstreamsPostSummary = "新增遠端術語上游";
    public const string AdminUpstreamsPostDetail = """
**Content-Type**：`application/json**

**欄位**：
- `systemUriPrefix`（必填）：比對 `Coding.system` 等之前綴；多筆時取**較長**前綴；無匹配時實作可能使用清單**第一筆**。
- `baseUrl`（必填）：上游 FHIR 基底，需含至 **`/fhir`**（例：`https://tx.example.com/fhir`）。
- `authorizationHeader`（選填）：原樣帶入 **Authorization** 標頭（勿寫入版本庫，建議用密鑰管理）。
- `timeoutSeconds`（選填，預設 60）

**成功**：HTTP 201。
""";

    public const string AdminUpstreamsDeleteSummary = "刪除遠端上游";
    public const string AdminUpstreamsDeleteDetail = "路徑 **id** 為上游列之 GUID。成功時 HTTP **204**。";

    public const string LookupSummary = "CodeSystem $lookup";
    public const string LookupDetail = """
FHIR **CodeSystem $lookup**（本實作為 **GET**）。

**必要查詢**：`code`、`system`；**選填**：`version`

**行為**：先查本機 SQLite 之 CodeSystem；必要時委派上游。

**成功**：`Parameters`（常含 `display`）。
""";

    public const string ValidateCodeCsSummary = "CodeSystem $validate-code";
    public const string ValidateCodeCsDetail = """
**必要**：`code`；**選填**：`url`（CodeSystem canonical）、`system`

本機有對應 CodeSystem 時直接驗證；否則委派上游。回傳 **`Parameters`**（含布林 **`result`**）。
""";

    public const string SubsumesSummary = "CodeSystem $subsumes";
    public const string SubsumesDetail = """
**必要查詢**：`codeA`、`codeB`、`system`；**選填**：`version`

需階層資料時於本機計算；無階層或無資料時語意見 **`Parameters`** 之 `outcome`。

""";

    public const string ExpandByUrlSummary = "ValueSet $expand（依 ValueSet.url）";
    public const string ExpandByUrlDetail = """
**查詢**：擇一提供本機已儲存之 ValueSet **`url`**，或使用 **`/ValueSet/{id}/$expand`**。

**選填**：`filter`、`offset`、`count`（分頁／篩選）。

本機可展開 **compose** 時直接計算；否則可委派上游 **`ValueSet/$expand`**。回傳 **ValueSet**（含 **expansion**）。
""";

    public const string ExpandByIdSummary = "ValueSet $expand（依邏輯 id）";
    public const string ExpandByIdDetail = """
路徑 **`id`** 為本機 ValueSet 之邏輯 **id**。選填查詢：`filter`、`offset`、`count`。

語意同「依 url 展開」，但以已儲存實例為準。
""";

    public const string ValidateCodeVsSummary = "ValueSet $validate-code";
    public const string ValidateCodeVsDetail = """
**必要**：`code`、`system`；**選填**：`url`（ValueSet canonical）、`display`。

若同時提供 **`profile`** 與 **`path`**，則依 **Binding 登錄** 解析 ValueSet 後再驗證（等同綁定語境）。

回傳 **`Parameters`**（`name=result` 之 **valueBoolean**）。
""";

    public const string TranslateSummary = "ConceptMap $translate";
    public const string TranslateDetail = """
**必要**：`code`、`system`；**選填**：`targetsystem`、`url`（ConceptMap canonical）。

本機有 **ConceptMap** 時先於 **`group`／`element`** 對照；無結果再委派上游。
""";

    public static string SearchSummary(string resourceType) => $"{resourceType} 類型搜尋（SEARCH）";
    public static string SearchDetail(string resourceType) => $"""
**GET** 類型層級搜尋 **`{resourceType}`**。

**查詢參數**（皆選填，多個條件為 **AND**）：`url`、`version`、`name`、`title`、`status`

**回應**：`Bundle`（`type=searchset`），含符合之 `{resourceType}` 資源。
""";

    public static string ReadSummary(string resourceType) => $"讀取單筆 {resourceType}（READ）";
    public static string ReadDetail(string resourceType) =>
        $"路徑中的 **id** 為該筆 {resourceType} 之邏輯 id。成功回傳 **{resourceType}** JSON（application/fhir+json）；不存在則 **404**。";

    public static string CreateSummary(string resourceType) => $"建立 {resourceType}（CREATE）";
    public static string CreateDetail(string resourceType) => $"""
**POST** 本文為完整 **`{resourceType}`** JSON，`Content-Type: application/fhir+json`，**resourceType** 須與路徑一致。

**成功**：通常 **201**，本文為建立後資源（含伺服器指派之 **id**）。
""";

    public static string UpdateSummary(string resourceType) => $"更新 {resourceType}（UPDATE）";
    public static string UpdateDetail(string resourceType) =>
        $"**PUT** /{resourceType}/「邏輯 id」，本文為完整資源 JSON。成功 **200**；無資源 **404**。";

    public static string DeleteSummary(string resourceType) => $"刪除 {resourceType}（DELETE）";
    public static string DeleteDetail(string resourceType) => $"""
成功時 HTTP **204**，無本文。
""";
}
