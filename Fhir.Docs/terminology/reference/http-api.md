# FHIR Terminology Service — HTTP API 速查

以下假設 **[base]** 為服務公開根 URL（例如 `https://tx.example.org`）。FHIR JSON 基底路徑為 **`[base]/fhir`**。

**Content-Type**：成功回應之 FHIR 資源／操作結果一般為 `application/fhir+json`。

### OpenAPI / Scalar（機制預設開啟）

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/openapi/v1.json` | OpenAPI 3 規格（JSON） |
| GET | `/scalar/v1` | Scalar 互動式 REST 文件 UI |

可經組態 **`Terminology:EnableApiReference`**（預設 `true`）關閉上述端點。詳見使用手冊 §4.6。

---

## 健全與能力

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/health` | 存活檢查，回應文字 `ok` |
| GET | `/fhir/metadata` | `CapabilityStatement`（省略 `mode` 時） |
| GET | `/fhir/metadata?mode=terminology` | `TerminologyCapabilities` |

---

## Terminology 操作（GET）

| 資源 | 路徑 | 必要／常用查詢參數 |
|------|------|-------------------|
| CodeSystem | `/fhir/CodeSystem/$lookup` | `code`、`system`；選填 `version` |
| CodeSystem | `/fhir/CodeSystem/$validate-code` | `code`；選填 `url`、`system` |
| CodeSystem | `/fhir/CodeSystem/$subsumes` | `codeA`、`codeB`、`system`；選填 `version` |
| ValueSet | `/fhir/ValueSet/$expand` | 擇一：`url` **或** 使用具體路徑 `/fhir/ValueSet/{id}/$expand`；選填 `filter`、`offset`、`count` |
| ValueSet | `/fhir/ValueSet/$validate-code` | `code`、`system`；選填 `url`、`display`；選填 **`profile`** + **`path`**（綁定語境，見使用手冊） |
| ConceptMap | `/fhir/ConceptMap/$translate` | `code`、`system`；選填 `targetsystem`、`url` |

---

## 資源 REST（CodeSystem／ValueSet／ConceptMap）

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/fhir/{ResourceType}?…` | **SEARCH**：支援 `url`、`version`、`name`、`title`、`status`（皆為選填，AND） |
| GET | `/fhir/{ResourceType}/{id}` | **READ** |
| POST | `/fhir/{ResourceType}` | **CREATE**，本文為 `application/fhir+json` |
| PUT | `/fhir/{ResourceType}/{id}` | **UPDATE** |
| DELETE | `/fhir/{ResourceType}/{id}` | **DELETE**，成功為 HTTP 204 |

---

## 管理 API（登錄／上游）

路徑前綴：`/fhir/admin/registry`。

### Bindings（FHIR Binding 登錄）

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/fhir/admin/registry/bindings` | `Bundle`（每筆 `Parameters` 含 `id`、`structureDefinition`、`path`、`valueSet`、`strength`） |
| POST | `/fhir/admin/registry/bindings` | JSON 本文，見使用手冊 |
| DELETE | `/fhir/admin/registry/bindings/{id}` | `id` 為 GUID |

### Upstreams（遠端術語伺服器）

| 方法 | 路徑 | 說明 |
|------|------|------|
| GET | `/fhir/admin/registry/upstreams` | `Bundle`（每筆 `Parameters` 含 `id`、`systemUriPrefix`、`baseUrl`、`timeoutSeconds`） |
| POST | `/fhir/admin/registry/upstreams` | JSON 本文，見使用手冊 |
| DELETE | `/fhir/admin/registry/upstreams/{id}` | `id` 為 GUID |

---

## Blazor 管理 UI（瀏覽器）

| 路徑 | 說明 |
|------|------|
| `/` | 首頁與說明連結 |
| `/admin/resources` | 已儲存之 CodeSystem／ValueSet／ConceptMap 列表 |
| `/admin/bindings` | 綁定登錄表單與預覽 |
| `/admin/upstreams` | 上游端點表單與預覽 |

詳細語意、錯誤碼與範例請見 [使用手冊](../user-manual.md)。
