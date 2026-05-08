# FHIR.Solutions — 代理與協作者指引

本檔為此儲存庫的**技術事實來源**（與 `.cursor/rules/` 互補：規則偏行為約束，此檔偏版本與邊界）。

## 為何不是 Cursor Skill（`.skill`）

- **Skill**：適合「可重複流程」（例如發版步驟、Hook 設定），通常放在使用者層級 skills 目錄。
- **本 repo**：目標框架、NuGet 邊界、專案對應關係會隨程式碼變更，應與程式**同庫版本化**，故使用 **`AGENTS.md` + `.cursor/rules/`**，而非獨立 skill 檔。

## 技術棧（統一基準）

| 項目 | 約定 |
|------|------|
| **.NET** | 程式庫與測試：**`net10.0`**（與 `Fhir.TypeFramework`、`Fhir.TypeFramework.Tests` 一致）。 |
| **C#** | `ImplicitUsings` / `Nullable`：**啟用**（與現有 `.csproj` 一致）。 |
| **方案檔** | `Fhir.Solution.slnx` |

### 已知例外（待收斂）

- **`Fhir.ResourceCreator`** 目標框架已為 **`net10.0`**；若使用 **Excel 模式**，仍依 **`System.Data.OleDb`**（主要為 Windows 情境）。Registry／StructureDefinition 主線可跨平台執行。

## 專案角色

| 專案 | 角色 |
|------|------|
| `Fhir.TypeFramework` | FHIR 強型別基底與資料型別；發佈 NuGet **`Fhir.TypeFramework`**。 |
| `Fhir.TypeFramework.Tests` | 單元／BDD 測試；**不**產生套件。 |
| `Fhir.ResourceCreator` | 由 FHIR 套件定義產生資源類別之工具；產出物應引用 **`Fhir.TypeFramework`**（單一套件策略，見 `.cursor/rules/fhir-sdk-architecture.mdc`）。詳見 **`Fhir.Docs/`**。 |

## Cursor 規則檔

- `/.cursor/rules/core-agent.mdc` — 全專案協作與引用格式  
- `/.cursor/rules/csharp-dotnet.mdc` — C# / 專案檔慣例  
- `/.cursor/rules/fhir-sdk-architecture.mdc` — TypeFramework／ResourceCreator 架構邊界  
- `/.cursor/rules/tech-stack.mdc` — 版本與對齊策略（給代理預設上下文）

變更 **目標框架** 或 **發佈套件邊界** 時，請同步更新本檔與 `tech-stack.mdc`。

## 與 Fhir.TypeFramework 的關係（產生之資源組件）

- 全方案僅維護**單一** `Fhir.TypeFramework` 套件；Primitive 層對齊 FHIR Normative 且預期可長期穩定、具向下相容語意。
- 由 `Fhir.ResourceCreator` 產生、並以 **每一組** Registry `package-id` + `version` 發佈的資源程式庫（預設命名如 `Fhir.Resources.R5`、`Fhir.Resources.R4`）僅涵蓋該 FHIR 線別之資源 POCO，**不是**跨版本共用的「Core」。跨版本共用程式碼只在 **`Fhir.TypeFramework`**。
- 上述資源組件應以 **`ProjectReference`（同儲存庫開發）** 或 **`PackageReference`（已發佈之 TypeFramework 版本）** 依賴該單一套件，勿依 FHIR 大版本拆多個 TypeFramework。
- JSON 與未來 XML 屬 I/O 邊界；記憶體內僅使用同一套 POCO 物件圖（見計畫書與 `Fhir.Resource.Tests.Common` 之註解）。
