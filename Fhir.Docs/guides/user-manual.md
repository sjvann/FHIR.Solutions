# Fhir.ResourceCreator 使用手冊

## 前置需求

- **.NET SDK**：與方案一致（目前資源組件與 TypeFramework 目標為 `net10.0`）。
- **網路**：Registry 模式需能連線 `packages.fhir.org`（或設定之備援 URL）以下載套件。
- **工作目錄**：請在 **`Fhir.ResourceCreator`** 專案目錄執行工具，以便 `appsettings.json`、`OutputRoot`、`PackageCacheDirectory` 等相對路徑正確。

## 設定檔

主要設定檔為 **`Fhir.ResourceCreator/appsettings.json`** 內之 **`Generator`** 區段。

| 鍵 | 說明 |
|----|------|
| `Mode` | `Registry`（主線）或 `Excel`（舊版 Excel） |
| `RegistryBaseUrl` | 主 Registry，預設 `https://packages.fhir.org` |
| `RegistryFallbackUrl` | 選用；主站失敗時改試 `packages2` 等 |
| `PackageCacheDirectory` | 解包快取根目錄，預設 `artifacts/fhir-packages` |
| `OutputRoot` | 產生專案根目錄，預設 `generated` |
| `RootNamespace` | 選填；留空時通常與推斷之組件名稱對齊（見命名文件） |
| `TypeFrameworkPackageVersion` | 發射之 `.csproj` 中 `Fhir.TypeFramework` **PackageReference** 版本 |
| `Packages` | 多筆套件目標，見下表 |

### `Packages[]` 每一筆

| 鍵 | 說明 |
|----|------|
| `PackageId` | NPM 套件 id，例如 `hl7.fhir.r5.core` |
| `Version` | 語意化版本，例如 `5.0.0` |
| `OutputProjectName` | 選填；資料夾名、組件名、NuGet **PackageId**。空則依 `PackageId` 推斷（如 `hl7.fhir.r5.core` → `Fhir.Resources.R5`） |
| `RootNamespace` | 選填；覆寫該套件產生類別的根命名空間 |
| `ResourcesInclude` | 選填；**非空**時僅產生列表內之資源類型名稱（如 `Patient`）。**空陣列**表示不篩選，由套件內所有符合條件之 SD 決定 |
| `ResourcesExclude` | 選填；排除之資源類型名稱 |

## 執行產生

```powershell
cd path\to\FHIR.Solutions\Fhir.ResourceCreator
dotnet run -c Release
```

成功時主控台會輸出 `Fhir.ResourceCreator finished.`。

### 產出目錄結構（範例）

```
generated/
  Fhir.Resources.R5/
    Fhir.Resources.R5.csproj
    Patient.cs
    Observation.cs
    ...
    Fhir.Resources.R5.Tests/
      Fhir.Resources.R5.Tests.csproj
      PatientSerializationTests.cs
      ...
```

## 建置與打包資源組件

於產出目錄（範例）：

```powershell
cd generated\Fhir.Resources.R5
dotnet build -c Release
dotnet pack -c Release
```

測試專案通常 **`IsPackable` 為 false**，僅供驗證，不發佈為 NuGet。

## 引用產生之組件

- **同儲存庫開發**：在應用專案對 `generated/.../Fhir.Resources.R5.csproj` 使用 **`ProjectReference`**（路徑依實際放置調整）。
- **已發佈 NuGet**：對套件 id（與 `OutputProjectName`／組件名一致）使用 **`PackageReference`**，並另 **`PackageReference` `Fhir.TypeFramework`**（版本與產生時鎖定策略一致）。

## 常見調整

| 需求 | 作法 |
|------|------|
| 僅產生部分資源 | 設定 `ResourcesInclude` 為類型名稱陣列 |
| 一次盡量全產 | `ResourcesInclude` 設為 `[]` 或省略鍵（若繫結模型預設為空） |
| R4 套件 | 新增另一筆 `Packages`，`PackageId` 如 `hl7.fhir.r4.core`，並設定適當 `OutputProjectName`／命名空間 |
| 避免「Core」誤解 | 組件／命名空間使用 `Fhir.Resources.R5` 這類 **線別**名稱，不要用後綴 `Core` 表示整包資源（見命名文件） |

## 故障排除

- **找不到套件／HTTP 錯誤**：檢查網路、`RegistryBaseUrl`／`RegistryFallbackUrl`、防火牆與 Proxy。
- **建置失敗（缺少複合型別）**：多數為 **TypeFramework** 尚未涵蓋某 FHIR datatype；需在 `Fhir.TypeFramework` 擴充後重新產生。
- **路徑錯亂**：確認於 `Fhir.ResourceCreator` 目錄執行，或改為設定 **絕對路徑** 之 `OutputRoot`／快取目錄。

更細設定鍵請見 [設定參考](../reference/configuration.md)。
