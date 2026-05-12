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

## THO 套件、種子與效能

- **批量更新**：建議以 FHIR NPM 套件 `hl7.terminology`（`.tgz`）匯入；CLI 專案 `Fhir.Terminology.PackageImporter` 會將 `package/` 內之 CodeSystem／ValueSet／ConceptMap 寫入 SQLite，並記錄於表 `TerminologyPackageImports`。官方瀏覽索引對照 URL 見程式碼 `ThoCatalogUrls`（codesystems／valuesets／external_code_systems 三頁）。
- **種子**：組態 `Terminology:AutoSkipCanonicalAndInternalStubsWhenPackageImportExists` 為 true 且資料庫已有套件匯入紀錄時，會跳過外部 canonical 與 internal FHIR slug 之 stub 種子，避免覆寫 THO 完整資源。
- **效能**：巨型 CodeSystem 鏡像後，`$lookup` 與依 compose 之 `$expand` 仍透過 `CodeSystemCodeIndex` 於載入時解析整份 JSON；若延遲過高，可另行評估離線 **概念列索引表**（未來遷移／優化項）。
