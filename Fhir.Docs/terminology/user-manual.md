# FHIR Terminology Service — 使用手冊

本文描述 **Fhir.Terminology.App** 對外服務之部署、組態、FHIR HTTP API、管理端點、Blazor 介面與錯誤語意，供整合客戶端與維運人員使用。規格對齊 **HL7 FHIR R5**；術語服務相關 SHALL 條目請以官方 [Terminology Service](https://www.hl7.org/fhir/terminology-service.html) 為準，本手冊說明**本實作**之行為與邊界。

---

## 1. 文件對象與範圍

| 對象 | 本手冊涵蓋 |
|------|------------|
| API 整合開發者 | `[base]/fhir` 下 REST／SEARCH、`$expand`／`$lookup`／`$validate-code`／`$subsumes`／`$translate`、`metadata` |
| 維運／部署 | SQLite 連線、環境變數、啟動與遷移、健康檢查 |
| 資料管理者 | Blazor 資源列表、綁定登錄、上游端點；或等同之 REST 管理 API |

**不在此詳述**：使用者身分驗證／授權（若於反向代理或閘道加 TLS、mTLS、OAuth，請依貴司基礎架構設定）。對遠端術語伺服器之 Bearer 等標頭可經上游組態傳遞（見 §8）。

---

## 2. 產品概述

- **角色**：輕量級 **FHIR Terminology** 端點，於 **SQLite** 保存 `CodeSystem`／`ValueSet`／`ConceptMap` 資源 JSON 與搜尋索引欄位，並可選擇性將請求 **委派**至相容 FHIR 之遠端術語伺服器（如 Ontoserver、Snowstorm 等）。
- **基底 URL**：對外應只有一個清楚的 **`[base]`**（例如 `https://terminology.example.org`）。本應用程式預設將 FHIR JSON API 掛在 **`[base]/fhir`**。
- **資料格式**：目前以 **FHIR JSON**（`application/fhir+json`）為主。

### 2.1 解析順序（本機優先）

對 **`$lookup`、`$validate-code`、`$subsumes`、`$translate`** 與本機可計算之 **`$expand`**：

1. **優先**使用 SQLite 中已儲存之資源完成運算。
2. 若本機無對應資源、或本機無法滿足（例如本地無 ConceptMap 對應列），再嘗試 **HTTP 委派**至已設定之 **upstream**（§8）。
3. 皆失敗時回傳 **`OperationOutcome`**（§10）。

---

## 3. 系統需求與建置

| 項目 | 說明 |
|------|------|
| 執行環境 | **.NET 10**（與專案 `TargetFramework` 一致） |
| 資料庫 | **SQLite**（檔案路徑由連線字串指定） |
| 網路 | 若使用遠端委派，應用程式須能連線至上游 Terminology Server |

於儲存庫根目錄建置：

```bash
dotnet build Fhir.Terminology.App/Fhir.Terminology.App.csproj -c Release
```

---

## 4. 部署與啟動

### 4.1 本機執行

```bash
cd Fhir.Terminology.App
dotnet run -c Release
```

預設 Kestrel 監聽埠請見終端機輸出（常見為 `http://localhost:5xxx`）。

### 4.2 組態來源優先序

與一般 ASP.NET Core 相同：**環境變數**（雙底線巢狀鍵）、**`appsettings.{Environment}.json`**、**User Secrets**（開發）、**命令列**，會覆寫 `appsettings.json`。

### 4.3 資料庫連線（必填）

連線字串鍵名：**`ConnectionStrings:Terminology`**。

預設（`appsettings.json`）：

```json
"ConnectionStrings": {
  "Terminology": "Data Source=terminology.db"
}
```

**環境變數範例（Linux／容器）**：

```bash
export ConnectionStrings__Terminology="Data Source=/data/terminology.db"
```

啟動時會驗證連線字串非空白；若未設定則 fallback 為 `Data Source=terminology.db`。

### 4.4 資料庫遷移

應用程式啟動時會對 **EF Core** 執行 **`MigrateAsync()`**，乾淨環境會自動建立結構。維運亦可於發版流程使用：

```bash
dotnet ef database update --project Fhir.Terminology.Infrastructure --startup-project Fhir.Terminology.App
```

（需已安裝 `dotnet-ef` 工具且專案含設計時 Factory。）

### 4.5 健康檢查

| 方法 | 路徑 | 成功回應 |
|------|------|----------|
| GET | `/health` | HTTP 200，本文 `ok` |

建議負載平衡器／Kubernetes **liveness** 使用此端點。

### 4.6 OpenAPI 文件與 Scalar（RESTful API 互動說明）

應用程式內建 **OpenAPI 3** 與 **[Scalar](https://scalar.com)** 介面，供對外整合時試打 HTTP、檢視查詢參數與路徑。

| 用途 | URL（相對於 `[base]`） |
|------|-------------------------|
| **Scalar 互動文件** | **`GET /scalar/v1`**（Scalar 預設文件路由為 `/scalar/{documentName}`，文件名稱為 **`v1`**） |
| **OpenAPI JSON** | **`GET /openapi/v1.json`** |
| **線上使用說明（Blazor）** | **`GET /docs`** 或 **`GET /manual`**（與本文互補之簡要手冊） |

- **端點說明**：各 API 於 OpenAPI 中已登錄 **Summary** 與 **Description**（繁中為主），於 Scalar 左側選擇操作後於右側展開即可閱讀；頂部 **Info** 亦有全文說明與標籤（Tags）群組。
- **關閉**：公開環境若不打算對外展示 API 瀏覽器，請設定 **`Terminology:EnableApiReference`** 為 **`false`**（`appsettings.Production.json` 或環境變數 **`Terminology__EnableApiReference=false`**）。關閉後 **`/scalar/*`** 與 **`/openapi/*`** 將不再註冊（**`/docs` 說明頁不受此開關影響**，仍可由網站存取）。
- **限制**：文件由 **`Microsoft.AspNetCore.OpenApi`** 依 Minimal API 自動產生；FHIR 資源本文多為動態 JSON，Schema 以框架推論為主；**完整語意仍以 FHIR 規格與本文 §6–§9 為準**。

---

## 5. FHIR 能力與 metadata

### 5.1 CapabilityStatement

- **請求**：`GET [base]/fhir/metadata`
- **回應**：HTTP 200，`Content-Type: application/fhir+json`，本文為 **`CapabilityStatement`**（伺服器能力宣告）。

### 5.2 TerminologyCapabilities

- **請求**：`GET [base]/fhir/metadata?mode=terminology`
- **回應**：HTTP 200，本文為 **`TerminologyCapabilities`**，反映本機已登錄之 CodeSystem 等（巨型詞彙若未匯入本機，請勿解讀為「本地含完整詞表」——通常需搭配 upstream，見 §11）。

---

## 6. 資源 REST 與 SEARCH

下列 **`ResourceType`** 為：`CodeSystem`、`ValueSet`、`ConceptMap`。

### 6.1 SEARCH（類型層級）

- **請求**：`GET [base]/fhir/{ResourceType}?url=&version=&name=&title=&status=`
- **語意**：查詢參數皆為**選填**；提供之多個條件為 **AND**。未帶參數則回傳該類型之符合集合（可能為空）。
- **回應**：HTTP 200，`Bundle`（`type=searchset`），内含符合之資源。

### 6.2 READ

- **請求**：`GET [base]/fhir/{ResourceType}/{id}`
- **成功**：HTTP 200，本文為該資源。
- **失敗**：HTTP 404（若路由未命中可能為其他中介層格式）。

### 6.3 CREATE

- **請求**：`POST [base]/fhir/{ResourceType}`
- **標頭**：`Content-Type: application/fhir+json`
- **本文**：完整 FHIR 資源 JSON；**`resourceType`** 須與路徑一致。
- **成功**：HTTP 201（實作回傳 `FhirOperationResult` 狀態碼），本文為建立後資源（含伺服器指派之 `id`）。
- **失敗**：HTTP 400 等，本文常為 **`OperationOutcome`**。

### 6.4 UPDATE

- **請求**：`PUT [base]/fhir/{ResourceType}/{id}`
- **本文**：完整資源 JSON；邏輯 id 須與路徑 `{id}` 一致（依伺服器實作）。
- **成功**：HTTP 200。
- **不存在**：HTTP 404。

### 6.5 DELETE

- **請求**：`DELETE [base]/fhir/{ResourceType}/{id}`
- **成功**：HTTP **204**，無本文。

---

## 7. Terminology 操作（SHALL 端點）

以下均為 **GET**（查詢參數傳遞）。完整路徑表亦可見 [HTTP API 速查](reference/http-api.md)。

### 7.1 CodeSystem `$lookup`

- **路徑**：`GET [base]/fhir/CodeSystem/$lookup`
- **必要查詢**：`code`、`system`
- **選填**：`version`
- **成功**：HTTP 200，`Parameters`（例如含 `display`）。
- **語意**：先於本機 CodeSystem 查碼；本機無資料或碼不在詞表時，可能委派 upstream（§2.1、§8）。

### 7.2 CodeSystem `$validate-code`

- **路徑**：`GET [base]/fhir/CodeSystem/$validate-code`
- **必要查詢**：`code`
- **選填**：`url`（CodeSystem canonical）、`system`
- **成功**：HTTP 200，`Parameters`（含布林 **`result`** 與訊息）。
- **語意**：本機有 CodeSystem 時直接驗證；否則委派。

### 7.3 CodeSystem `$subsumes`

- **路徑**：`GET [base]/fhir/CodeSystem/$subsumes`
- **必要查詢**：`codeA`、`codeB`、`system`
- **選填**：`version`
- **成功**：HTTP 200，`Parameters`（`outcome` 程式碼）。
- **語意**：需階層資料時於本機計算；本機無 CodeSystem 時委派。

### 7.4 ValueSet `$expand`

- **路徑（擇一）**  
  - `GET [base]/fhir/ValueSet/$expand?url=…`  
  - `GET [base]/fhir/ValueSet/{id}/$expand`
- **選填**：`filter`、`offset`、`count`
- **成功**：HTTP 200，本文為 **`ValueSet`**（含 **`expansion`**）。
- **語意**：若 ValueSet 於本機且 **compose** 可於本機展開則直接計算；否則（或失敗後）可委派 upstream。

### 7.5 ValueSet `$validate-code`

- **路徑**：`GET [base]/fhir/ValueSet/$validate-code`
- **必要查詢**：`code`、`system`
- **選填**：`url`（ValueSet canonical）、`display`
- **綁定語境（FHIR Binding 登錄）**：若同時提供 **`profile`** 與 **`path`**，伺服器會依綁定登錄（§9）解析 **ValueSet canonical**，再執行與一般 `$validate-code` 相同之驗證語意。
  - **`profile`**：StructureDefinition URL（或貴組織約定之 Profile URL）。
  - **`path`**：元素路徑（例如 `Patient.gender`），須與登錄時一致。
- **成功**：HTTP 200，`Parameters`（`parameter.name=result` 之 **`valueBoolean`**）。

### 7.6 ConceptMap `$translate`

- **路徑**：`GET [base]/fhir/ConceptMap/$translate`
- **必要查詢**：`code`、`system`
- **選填**：`targetsystem`、`url`（ConceptMap canonical）
- **成功**：HTTP 200，`Parameters`（匹配時含 `match` 與 `concept`）。
- **語意**：本機有 ConceptMap 時先於本地對照；無結果再委派。

---

## 8. 遠端上游（Upstream）

### 8.1 用途

當本機 **無對應資源**或本機無法完成操作時，若已設定上游，客戶端請求可能以 **同一 FHIR 操作路徑與查詢字串**轉發至上游（例如 `GET .../ValueSet/$expand?url=...`）。

### 8.2 設定方式

透過管理 API（§9）或同等資料寫入 **`UpstreamServers`** 表。語意欄位：

| 欄位 | 說明 |
|------|------|
| **systemUriPrefix** | 比對 **Coding.system** 或路由時之前綴（較長者優先）。若請求無法由此匹配，實作可能退回使用清單中**第一筆**上游。 |
| **baseUrl** | 上游 FHIR **基底**，應含至 **`/fhir`** 之前綴（例如 `https://tx.company.com/fhir`），實際請求為 `baseUrl` + 相對路徑。 |
| **authorizationHeader** | 選填；若設定則以 HTTP 標頭 **`Authorization`** 原樣送出（請勿將機敏資訊寫進版本庫；建議環境變數／Secret）。 |
| **timeoutSeconds** | 該上游請求逾時（秒）；未給或無效時預設 **60**。 |

### 8.3 HttpClient

全域 **`HttpClient`** 名稱於程式中為 **`terminology-upstream`**（維運追蹤日誌時可引用）；預設逾時 **120 秒**（與單一上游 `timeoutSeconds` 不同層級時請以實際程式為準）。

---

## 9. FHIR Binding 登錄與管理 API

### 9.1 概念

**Binding 登錄**用於記錄「某 **StructureDefinition（Profile）URL** + **元素 path**」對應到哪個 **ValueSet canonical** 與 **binding strength**」，以便：

- 使用 **`$validate-code?profile=&path=`** 時自動選定 ValueSet；
- Blazor 頁面維護與稽核。

### 9.2 REST 端點（非標準 FHIR 資源，為本產品擴充）

前綴：**`/fhir/admin/registry`**。

#### GET `/fhir/admin/registry/bindings`

- **回應**：`Bundle`，各 `entry.resource` 為 **`Parameters`**，建議包含：`id`、`structureDefinition`、`path`、`valueSet`、`strength`。

#### POST `/fhir/admin/registry/bindings`

- **Content-Type**：`application/json`
- **本文（camelCase）**：

```json
{
  "structureDefinitionUrl": "http://hl7.org/fhir/StructureDefinition/Patient",
  "elementPath": "Patient.gender",
  "valueSetCanonical": "http://hl7.org/fhir/ValueSet/administrative-gender",
  "strength": "required"
}
```

- **必填**：`valueSetCanonical`、`strength`
- **成功**：HTTP **201**，本文為 `Parameters`（通常含新建 **`id`**）。

#### DELETE `/fhir/admin/registry/bindings/{id}`

- **路徑**：`id` 為綁定列之 **GUID**。
- **成功**：HTTP **204**。

#### GET／POST／DELETE `/fhir/admin/registry/upstreams`

- **POST 本文範例**：

```json
{
  "systemUriPrefix": "http://snomed.info/sct",
  "baseUrl": "https://your-tx-server.example/fhir",
  "authorizationHeader": "Bearer ***",
  "timeoutSeconds": 120
}
```

- **必填**：`systemUriPrefix`、`baseUrl`
- **成功**：POST 為 **201**（含新 `id`）；DELETE 成功為 **204**。

---

## 10. 錯誤處理與 HTTP 狀態碼

| 情況 | 常見 HTTP | 本文 |
|------|------------|------|
| 參數不足／無效 | 400 | **`OperationOutcome`**（或簡短文字於部分中介層） |
| 資源或登錄不存在 | 404 | **`OperationOutcome`** |
| 伺服器內部錯誤 | 500 | **`OperationOutcome`** |

**OperationOutcome** 之 `issue.code`、`issue.diagnostics` 應作為客戶端除錯依據；生產環境請避免將機敏資料寫入 `diagnostics`。

---

## 11. 合規與已知限制

- **巨型詞彙**（如 SNOMED CT、LOINC 全集）：**不預期**以 SQLite 承載完整詞庫；請透過 **upstream** 與／或 **Binding 登錄**管理 canonical 與驗證語境。
- **ValueSet compose**：本機僅支援可於現有 CodeSystem 資料上完成的展開子集；複雜組合或大型語意請委派專用術語伺服器。
- **誠實宣告**：請將 `CapabilityStatement`／`TerminologyCapabilities` 與實際部署之上游／資料同步維護；內部對照表可見 [`Fhir.Terminology.App/TerminologyConformance.md`](../../Fhir.Terminology.App/TerminologyConformance.md)。

---

## 12. Blazor 管理介面

| 瀏覽器路徑 | 功能摘要 |
|------------|----------|
| `/` | 首頁與 REST 基底說明 |
| `/admin/resources` | 列出已儲存之 CodeSystem／ValueSet／ConceptMap（索引欄） |
| `/admin/bindings` | 檢視 Bundle JSON、表單新增綁定 |
| `/admin/upstreams` | 檢視上游 Bundle、表單新增上游 |

Blazor 使用 **Interactive Server** 模式；請於正式環境設定 **WebSocket**／**sticky session**（若經反向代理）。

---

## 13. 可觀測性與疑難排解

- **結構化日誌**：委派至上游時會記錄轉發 URL 等資訊（層級依 `Logging` 組態）。
- **常見問題**  
  - **404／OperationOutcome not-found**：本機無資源且無 upstream、或 upstream URL 錯誤。  
  - **上游逾時**：調整 `timeoutSeconds` 或網路；檢查 TLS／憑證。  
  - **SQLite 鎖定**：單檔 SQLite 不適合極高併發寫入；可改檔案位置或擴展部署策略。

---

## 14. 附錄：組態範本（Ontoserver／內部 Tx）

以下為 **`appsettings.Production.json`** 片段範例（請勿將機敏資料提交版本庫）：

```json
{
  "ConnectionStrings": {
    "Terminology": "Data Source=/var/lib/fhir-terminology/app.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Fhir.Terminology": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

上游 **`authorizationHeader`** 建議於部署後經 **REST POST** 寫入資料庫，或由未來版本之 Secret 注入機制提供（現行為資料庫欄位存放）。

---

## 15. 附錄：範例（curl）

設 **`BASE=http://localhost:5000`**（請替換為實際 `[base]`）。

```bash
# CapabilityStatement
curl -sS "$BASE/fhir/metadata" -H "Accept: application/fhir+json"

# TerminologyCapabilities
curl -sS "$BASE/fhir/metadata?mode=terminology" -H "Accept: application/fhir+json"

# 建立小型 CodeSystem 後 ValueSet $expand（路徑依你的 id）
curl -sS "$BASE/fhir/ValueSet/my-vs-id/\$expand" -H "Accept: application/fhir+json"

# 綁定語境驗證（需先 POST 綁定登錄）
curl -sS "$BASE/fhir/ValueSet/\$validate-code?profile=http%3A%2F%2Fhl7.org%2Ffhir%2FStructureDefinition%2FPatient&path=Patient.gender&code=male&system=http%3A%2F%2Fhl7.org%2Ffhir%2Fadministrative-gender" \
  -H "Accept: application/fhir+json"
```

注意：於 shell 中 **`$`** 需跳脫為 **`\$`**，以免被變數展開。

---

## 16. 版本與文件維護

- **FHIR 版本**：本應用程式目標為 **R5**（與 `Fhir.Resources.R5` 一致）。
- **文件位置**：`Fhir.Docs/terminology/`（與程式同庫維護；行為以發行版本之程式碼為準）。
