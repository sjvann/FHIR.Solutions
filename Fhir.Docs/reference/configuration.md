# Generator 設定參考

設定來源：**`Fhir.ResourceCreator/appsettings.json`** 之 **`Generator`** 節段（見 `GeneratorOptions.SectionName`）。

## 根層屬性

| 屬性 | 型別 | 預設 | 說明 |
|------|------|------|------|
| `Mode` | 列舉字串 | `Registry` | `Registry` 或 `Excel` |
| `RegistryBaseUrl` | string | `https://packages.fhir.org` | 主 NPM Registry |
| `RegistryFallbackUrl` | string? | `https://packages2.fhir.org` | 失敗時備援；可設為 null 停用 |
| `PackageCacheDirectory` | string | `artifacts/fhir-packages` | 下載與解包快取（相對於目前工作目錄） |
| `OutputRoot` | string | `generated` | 產生專案根目錄 |
| `ExcelDefinitionsPath` | string? | null | 僅 Excel 模式：含 `*.xlsx` 之資料夾 |
| `RootNamespace` | string | `""` | 全域命名空間前綴；空則通常採用推斷之組件名稱 |
| `TypeFrameworkPackageVersion` | string | `1.0.0` | 發射專案中 NuGet `Fhir.TypeFramework` 版本字串 |
| `Packages` | 陣列 | `[]` | 多套件產生目標 |

## `Packages[]` 元素

| 屬性 | 說明 |
|------|------|
| `PackageId` | 必填（Registry 模式實務上）；Registry 套件 id |
| `Version` | 必填；套件版本 |
| `OutputProjectName` | 選填；空則依 `GeneratedResourceNaming.SuggestOutputProjectName(PackageId)` 推斷 |
| `RootNamespace` | 選填；覆寫該套件之 POCO 根命名空間 |
| `ResourcesInclude` | 選填；**有任一項目時**僅產生所列資源類型；**空清單**表示不以此篩選 |
| `ResourcesExclude` | 選填；排除之類型名稱 |

## 篩選語意（重要）

Orchestrator 邏輯摘要：

- `ResourcesInclude.Count > 0`：僅處理名稱出現在列表中之資源類型。
- `ResourcesInclude` 為空：不套用「僅包含」篩選（仍受 SD 條件與 exclude 影響）。
- `ResourcesExclude`：永遠排除所列類型。

## Excel 模式額外說明

`Mode` 為 `Excel` 時需設定 **`ExcelDefinitionsPath`**；命名空間若未設定全域 `RootNamespace`，管線可能使用預設後備（見程式碼 `RunExcelLegacy`）。Registry 模式為建議主線。

## 與程式碼對應

- 設定類別：`Fhir.ResourceCreator/Configuration/GeneratorOptions.cs`
- 命名推斷：`Configuration/GeneratedResourceNaming.cs`
