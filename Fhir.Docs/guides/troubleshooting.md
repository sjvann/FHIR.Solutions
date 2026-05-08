# 疑難排解與常見問題

## 產生器執行後沒有預期檔案

- 確認於 **`Fhir.ResourceCreator`** 目錄執行 `dotnet run`，使 `OutputRoot`、`PackageCacheDirectory` 相對路徑正確。
- 檢查 **`ResourcesInclude`**：若為非空陣列，僅會產生列舉之類型；若為 **空陣列 `[]`**，表示依套件內 SD 全量篩選（仍排除 abstract、無 snapshot 等）。

## HTTP／Registry 錯誤

- 確認可連線 **`RegistryBaseUrl`**；必要時設定 **`RegistryFallbackUrl`**（例如 `packages2.fhir.org`）。
- 企業環境檢查 Proxy、憑證與防火牆。

## 建置產生之 `.csproj` 失敗

| 錯誤型態 | 處理方向 |
|----------|----------|
| 找不到 FHIR 複合型別（資料型別） | 於 **`Fhir.TypeFramework`** 補上對應類別後重新建置／重新產生 |
| `Range` 與 `System.Range` 混淆 | 產生器應已對 FHIR `Range` 使用完整限定；若再出現，請回報產生器版本 |
| 重複屬性名稱 | 通常為 snapshot 重複路徑或 choice 重複；產生器已做去重，若仍發生請附 **資源類型名稱** 與 SD 片段 |

## 測試失敗

- 先 **`dotnet build`** 資源組件專案，再 **`dotnet test`** 測試專案。
- Smoke 測試僅驗證最小 JSON；若實際交換格式較複雜，請另外加入 **fixture** 與自訂断言（見 [測試手冊](testing-manual.md)）。

## Excel 模式

- 需本機可讀取 **`ExcelDefinitionsPath`** 下之 **`.xlsx`**，且 OleDb 環境可用（多為 Windows）。
- 與 Registry 主線產物可能不一致；新功能以 Registry 為準。

## 取得協助時建議附上的資訊

- `appsettings.json` 中 **`Generator`** 區段（可遮罩敏感 URL）。
- 失敗時之 **完整建置／測試輸出**。
- 涉及之 **資源類型名稱**（如 `Observation`）與 **FHIR 套件 id／版本**。
