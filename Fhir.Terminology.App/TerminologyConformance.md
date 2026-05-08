# FHIR Terminology Service — Conformance 對照（R5）

依 [Terminology Service](https://www.hl7.org/fhir/terminology-service.html) **SHALL** 條目與本實作對照（審查用；行為以程式為準）。

| SHALL／能力 | 本機 SQLite | 遠端委派（Upstream） | 備註 |
|-------------|-------------|----------------------|------|
| REST READ／SEARCH（CodeSystem／ValueSet／ConceptMap） | 是 | 不適用 | `url`／`version`／`name`／`title`／`status` |
| GET `metadata`（CapabilityStatement） | 是 | 不適用 | `mode` 省略時 |
| GET `metadata?mode=terminology`（TerminologyCapabilities） | 是（本機已存 CodeSystem） | 否 | 巨型詞彙未匯入時不宣稱完整詞庫 |
| CodeSystem `$lookup` | 是（已存概念） | 是（有 upstream） | 無資料且無 upstream → OperationOutcome |
| CodeSystem `$validate-code` | 是 | 是 | 同上 |
| CodeSystem `$subsumes` | 需階層資料（索引／concept 樹） | 是 | 無階層時本機回覆 not-applicable／或委派 |
| ValueSet `$expand` | compose 可於本機展開 | 是 | 本機失敗或無資源時嘗試上游 |
| ValueSet `$validate-code` | 依展開結果 | 是 | 可選 `profile`+`path` 走 Binding 登錄 |
| ConceptMap `$translate` | 本機對照表 | 是 | |
| FHIR Binding 登錄 | 表 `BindingRegistry` + REST／Blazor | — | 依 StructureDefinition + path 選 ValueSet 再驗證 |

**解析順序**：對 `$lookup`／`$validate-code`／`$subsumes`／`$translate`／本機可算的 `$expand`，優先使用 **SQLite 本機**；僅在資源不存在於本機、或本機無法滿足（例如 ConceptMap 無對應）時再 **HTTP 委派**至已設定之上游。
