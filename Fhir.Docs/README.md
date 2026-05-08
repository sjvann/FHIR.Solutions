# FHIR.Solutions 文件索引

本目錄收錄與 **FHIR 資源產生**、**型別框架**及**測試策略**相關之說明，與程式碼同庫維護。

| 類別 | 路徑 | 說明 |
|------|------|------|
| 整體架構 | [architecture/overview.md](architecture/overview.md) | 元件角色、資料流、與 NuGet 邊界 |
| 使用手冊 | [guides/user-manual.md](guides/user-manual.md) | 設定檔、執行產生器、輸出目錄與命名 |
| 測試手冊 | [guides/testing-manual.md](guides/testing-manual.md) | 雙層測試、本機驗證指令 |
| 疑難排解 | [guides/troubleshooting.md](guides/troubleshooting.md) | Registry、建置、測試常見問題 |
| 設定參考 | [reference/configuration.md](reference/configuration.md) | `Generator` 區段鍵值說明 |
| 命名與發佈 | [reference/naming-and-packaging.md](reference/naming-and-packaging.md) | `Fhir.Resources.R5` 等命名約定、`dotnet pack` |

相關程式位置：`Fhir.ResourceCreator`、`Fhir.TypeFramework`、`Fhir.Resource.Tests.Common`，以及產物目錄 `Fhir.ResourceCreator/generated/`（預設）。

另見儲存庫根目錄 `AGENTS.md`（代理／協作者技術邊界摘要）。
