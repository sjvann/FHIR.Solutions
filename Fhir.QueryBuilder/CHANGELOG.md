# 更新日誌

FHIR Query Builder 專案的所有重要變更都將記錄在此檔案中。

格式基於 [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)，
此專案遵循 [語意化版本](https://semver.org/spec/v2.0.0.html)。

## [未發布]

## [2.0.0] - 2024-12-19

### 🚀 主要新增功能
- **進階搜尋 UI**: 全新專用進階搜尋頁籤，具有直觀控制項
- **鏈式搜尋**: 支援跨資源搜尋 (例如: `patient.name=John`)
- **反向鏈式搜尋**: 尋找被其他資源參考且符合特定條件的資源
- **複合參數**: 使用 `$` 分隔符的多組件搜尋參數
- **過濾表達式**: 使用 FHIR Filter 語法進行進階過濾
- **增強結果控制**: 分頁、總數、摘要和元素選擇

### 🎯 進階搜尋功能
- **CompositeParameterBuilder**: 用於複合搜尋的新參數建構器
- **FilterParameterBuilder**: 用於過濾表達式的新參數建構器
- **AdvancedSearchControl**: 專用於進階搜尋功能的 UI 控制項
- **擴展方法**: 用於建構複雜參數的實用方法
- **即時更新**: 參數變更時查詢 URL 自動更新
- **參數驗證**: 進階參數的內建驗證

### 🔧 FluentApi 增強
- `Chain(string chainPath, string value)` - 鏈式搜尋支援
- `ReverseChain(string resourceType, string searchParam, string value)` - 反向鏈式搜尋
- `WhereComposite(string parameterName, params string[] components)` - 複合參數
- `Filter(string filterExpression)` - 過濾表達式
- `Offset(int offset)` - 分頁偏移
- `Total(string totalMode)` - 總數控制
- `Contained(string containedMode)` - 包含資源控制

### 📚 文件與範例
- **ADVANCED_FEATURES.md**: 進階功能的完整指南
- **醫療場景**: 真實世界的醫療保健範例
- **API 文件**: 使用新方法和參數更新
- **使用範例**: 逐步 UI 和程式化範例

### 🏗️ 架構改進
- 使用現代模式進行全面架構重構
- 使用 ViewModels 實作 MVVM 模式
- 具有服務註冊的依賴注入容器
- 用於程式化查詢建構的流暢 API
- 查詢參數建構器的策略模式
- 參數建構器建立的工廠模式
- 具有可設定過期時間的進階快取系統
- 效能監控和指標收集
- 具有事件驅動通知的錯誤處理服務
- 長時間執行操作的進度追蹤服務
- 支援環境變數的設定管理
- 具有內建範本的查詢範本系統
- 查詢歷史追蹤和管理
- 匯出功能 (JSON、XML、CSV 格式)
- 用於改善效能的連線池
- 結果分頁支援
- 具有單元和整合測試的完整測試框架
- 用於容器化部署的 Docker 支援
- 使用 GitHub Actions 的 CI/CD 管道
- 完整的文件和部署腳本

### 變更
- 全面從同步操作遷移到非同步操作
- 使用非阻塞操作改善 UI 回應性
- 使用詳細錯誤資訊增強錯誤處理
- 使用驗證改善設定管理
- 使用結構化日誌支援改善日誌記錄
- 使用可設定 SSL 驗證增強安全性

### 修正
- HTTP 客戶端使用中的記憶體洩漏
- 並發操作中的執行緒安全問題
- 設定載入邊界情況
- 長時間操作期間的 UI 凍結

## [1.0.0] - 2024-01-15

### 新增
- FHIR Query Builder 初始版本
- 用於查詢建構的基本 Windows Forms UI
- FHIR 伺服器連接功能
- 資源類型選擇和參數建構
- 具有 JSON 結果顯示的查詢執行
- 基本匯出功能
- 簡單設定管理
- 連接到 FHIR R4 和 R5 伺服器
- 常見類型的搜尋參數支援
- Include 和 RevInclude 參數支援
- 基本錯誤處理和日誌記錄

### 功能
- 視覺化查詢建構器介面
- 支援字串、日期、數字、權杖和參考參數
- 即時查詢 URL 產生
- 具有樹狀結構的 JSON 結果檢視器
- 將結果匯出到檔案
- 最近伺服器歷史
- 基本 OAuth 支援偵測

### 技術
- .NET 8.0 Windows Forms 應用程式
- 與 FhirServerAgent 函式庫整合
- 設定的 JSON 序列化
- 用於 FHIR 伺服器通訊的 HTTP 客戶端
- 基本日誌記錄到控制台和檔案

## [0.9.0] - 2023-12-01

### 新增
- 用於測試的 Beta 版本
- 核心查詢建構功能
- 基本 FHIR 伺服器整合
- 用於參數輸入的簡單 UI
- JSON 結果顯示

### 已知問題
- 有限的錯誤處理
- 沒有快取機制
- 同步操作導致 UI 凍結
- 僅基本設定選項

## [0.1.0] - 2023-10-15

### 新增
- 初始概念驗證
- 基本 FHIR 連接
- 簡單查詢執行
- 最小 UI 實作

---

## 版本歷史摘要

- **v1.0.0**: 具有核心功能的初始穩定版本
- **v1.1.0**: 使用現代模式進行主要架構重構
- **v1.2.0**: 效能改進和進階功能
- **v2.0.0**: 進階搜尋功能和 UI 增強

## 遷移指南

### 從 v1.0.0 到 v2.0.0

v2.0.0 版本包含重大的功能增強。UI 保持向後相容，但新增了強大的進階搜尋功能。

#### 重大變更
- 設定檔格式已擴展 (向後相容)
- 新增進階搜尋 API (不影響現有功能)
- 最低 .NET 版本需求更新至 8.0

#### 可用的新功能
- 用於程式化查詢建構的流暢 API
- 查詢範本和歷史
- 進階匯出選項
- 效能監控
- 增強錯誤處理

#### 遷移步驟
1. 備份您現有的設定和查詢歷史
2. 安裝新版本
3. 應用程式將自動遷移您的設定
4. 檢查 appsettings.json 中的新設定選項
5. 探索查詢範本和匯出選項等新功能

### 設定遷移

仍支援舊設定格式，但提供新選項:

```json
{
  "FhirQueryBuilder": {
    // 現有設定保持不變
    "DefaultServerUrl": "https://server.fire.ly",

    // 新增的部分
    "Ui": {
      "Theme": "Light",
      "ShowAdvancedOptions": false
    },
    "Performance": {
      "MaxConcurrentRequests": 5,
      "EnableResultStreaming": false
    },
    "Security": {
      "ValidateSslCertificates": true,
      "RequireHttps": true
    }
  }
}
```

## 支援和相容性

### 支援的平台
- Windows 10 版本 1809 或更新版本
- Windows 11
- Windows Server 2019 或更新版本

### 支援的 FHIR 版本
- FHIR R4 (4.0.1)
- FHIR R5 (5.0.0)

### .NET 需求
- .NET 8.0 Runtime (包含在自包含部署中)
- Visual C++ Redistributable (用於某些原生相依性)

## 貢獻

我們歡迎貢獻！請參閱我們的[貢獻指南](CONTRIBUTING.md)了解以下詳情:
- 如何回報錯誤
- 如何建議增強功能
- 開發環境設定
- 程式碼風格指南
- Pull request 流程

## 授權

本專案採用 MIT 授權 - 詳情請參閱 [LICENSE](LICENSE) 檔案。
