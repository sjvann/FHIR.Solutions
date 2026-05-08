# FHIR Terminology Service 文件

對外 **FHIR R5 Terminology** HTTP 服務（SQLite 儲存、SHALL 術語操作、選配遠端委派）之正式文件入口。

| 文件 | 說明 |
|------|------|
| [**使用手冊（完整）**](user-manual.md) | 部署、組態、REST／操作、管理 API、Blazor、綁定與上游、錯誤處理、範例請求 |
| [HTTP API 速查](reference/http-api.md) | 端點、方法、查詢參數一覽表；含 **Scalar／OpenAPI** 路徑 |

程式位置：`Fhir.Terminology.App`（主機）、`Fhir.Terminology.Core`、`Fhir.Terminology.Infrastructure`、`Fhir.Terminology.Tests`。

與實作對照之合規摘要亦可參考程式庫內 [`Fhir.Terminology.App/TerminologyConformance.md`](../../Fhir.Terminology.App/TerminologyConformance.md)。
