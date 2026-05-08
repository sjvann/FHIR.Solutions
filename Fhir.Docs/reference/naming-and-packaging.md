# 命名與 NuGet 發佈約定

## 為何避免在資源組件名稱使用「Core」

在 .NET 生態中，**Core** 常被理解為「多版本／多套件共用的基礎層」。本方案中：

- **跨 FHIR 版本共用**的 primitives 與基底型別集中在 **`Fhir.TypeFramework`**。
- **依 Registry 套件版本產生**的資源 POCO 應使用 **線別／版本意涵**清楚的名稱，例如 **`Fhir.Resources.R5`**、`Fhir.Resources.R4`，而非 `Fhir.Resources.R5.Core`。

以免 SDK 使用者誤以為「Core」底下是所有版本共用的資源定義。

## 預設組件與命名空間

當 **`OutputProjectName`** 未設定時，可由 **`PackageId`** 推斷，例如：

- `hl7.fhir.r5.core` → `Fhir.Resources.R5`
- `hl7.fhir.r4.core` → `Fhir.Resources.R4`

glob **`RootNamespace`** 留空時，通常與推斷出的 **`OutputProjectName`** 對齊，使資料夾、組件名、根命名空間一致。

## 產出與 NuGet

發射之 **`{OutputProjectName}.csproj`** 通常包含：

- **`PackageId`**：等同組件名（如 `Fhir.Resources.R5`）
- **`PackageVersion`**：對應設定之套件版本（或專案慣例）
- **`PackageReference` `Fhir.TypeFramework`**：版本取自 `TypeFrameworkPackageVersion`，除非同 repo 開發改為 **ProjectReference**

於產出目錄執行：

```powershell
dotnet pack -c Release
```

可取得資源組件 **`.nupkg`**；測試專案預設 **不打包**。

## 儲存庫開發 vs 已發佈套件

| 情境 | TypeFramework 引用方式 |
|------|-------------------------|
| 同 repo、本機建置 | `ProjectReference` 指向 `Fhir.TypeFramework/Fhir.TypeFramework.csproj`（若發射器偵測到 repo 根） |
| 僅使用 NuGet | `PackageReference` + `TypeFrameworkPackageVersion` |

## 延伸閱讀

- `AGENTS.md`（儲存庫根目錄）
- [整體架構](../architecture/overview.md)
