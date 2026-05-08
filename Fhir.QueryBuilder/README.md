# FHIR 查詢建構器

一個功能完整的 Windows Forms 應用程式，用於建構、測試和管理 FHIR 查詢，具備進階功能包括快取、效能監控和匯出功能。

## 🚀 最新更新 (v2.0)

### ✨ 全新進階搜尋功能
- **鏈式搜尋 (Chaining)**: 跨相關資源搜尋 (例如: `patient.name=John`)
- **反向鏈式搜尋 (Reverse Chaining)**: 尋找參考特定條件資源的其他資源
- **複合參數 (Composite)**: 複雜的多組件搜尋參數
- **過濾表達式 (Filter)**: 使用 FHIR Filter 語法進行進階過濾
- **增強結果控制**: 分頁、總數、摘要和元素選擇

### 🎯 主要改進
- **現代化 UI**: 全新進階搜尋頁籤，直觀易用的控制項
- **即時更新**: 新增參數時查詢 URL 自動更新
- **參數驗證**: 內建參數驗證和錯誤處理
- **實用範例**: 完整的醫療場景使用範例
- **可擴展架構**: 模組化設計，易於新增功能

## 功能特色

### 核心功能
- **視覺化查詢建構器**: 直觀的介面用於建構 FHIR 查詢
- **流暢 API**: 支援方法鏈式的程式化查詢建構
- **伺服器連接**: 連接任何 FHIR R4/R5 伺服器
- **資源瀏覽器**: 瀏覽支援的資源和搜尋參數
- **查詢執行**: 執行查詢並即時顯示結果
- **結果視覺化**: JSON 樹狀檢視和格式化文字顯示

### 進階功能
- **查詢範本**: 內建和自訂查詢範本
- **查詢歷史**: 追蹤和重複使用先前的查詢
- **匯出功能**: 匯出結果為 JSON、XML、CSV 格式
- **效能監控**: 追蹤查詢效能和伺服器指標
- **快取系統**: 智慧快取以提升效能
- **錯誤處理**: 全面的錯誤追蹤和報告
- **進度追蹤**: 長時間操作的即時進度更新

### 🆕 新增進階搜尋功能
- **鏈式搜尋 (Chaining Search)**
  - 語法: `patient.name=John`
  - 用途: 搜尋特定患者的觀察記錄
  - 範例: `Observation?patient.organization.name=General Hospital`

- **反向鏈式搜尋 (Reverse Chaining)**
  - 語法: `_has:ResourceType:searchParam:value`
  - 用途: 尋找有特定檢驗結果的患者
  - 範例: `Patient?_has:Observation:patient:code=8480-6`

- **複合參數 (Composite Parameters)**
  - 語法: `param=component1$component2$component3`
  - 用途: 複雜的多組件搜尋
  - 範例: `component-code-value-quantity=http://loinc.org|8480-6$120$mmHg`

- **過濾表達式 (Filter Expressions)**
  - 語法: FHIR Filter 語法
  - 用途: 複雜的條件組合
  - 範例: `_filter=name co 'John' and birthDate ge 1990-01-01`

- **結果控制參數**
  - `_count`: 每頁結果數量
  - `_offset`: 分頁偏移量
  - `_total`: 總數控制 (none/estimate/accurate)
  - `_summary`: 摘要模式 (true/false/text/data/count)
  - `_elements`: 指定要包含的元素

### 架構亮點
- **依賴注入**: 現代化 IoC 容器與服務註冊
- **MVVM 模式**: 使用 ViewModels 清晰分離關注點
- **策略模式**: 可插拔的查詢參數建構器
- **工廠模式**: 可擴展的參數建構器工廠
- **非同步/等待**: 全程非阻塞操作
- **配置管理**: 靈活的配置與環境支援

## 開始使用

### 系統需求
- .NET 8.0 或更新版本
- Windows 10/11
- Visual Studio 2022 (建議) 或 VS Code

### 安裝步驟

1. 複製儲存庫：
```bash
git clone https://github.com/sjvann/FhirServerHelper.git
cd FhirServerHelper/FHIRQueryBuilder
```

2. 還原相依性：
```bash
dotnet restore
```

3. 建置應用程式：
```bash
dotnet build
```

4. 執行應用程式：
```bash
dotnet run
```

### 配置設定

應用程式使用 `appsettings.json` 進行配置。主要設定包括：

```json
{
  "FhirQueryBuilder": {
    "DefaultServerUrl": "https://server.fire.ly",
    "RequestTimeoutSeconds": 30,
    "EnableCaching": true,
    "CacheExpirationMinutes": 30,
    "Ui": {
      "Theme": "Light",
      "ShowAdvancedOptions": false
    },
    "Performance": {
      "MaxConcurrentRequests": 5,
      "MaxResultsPerPage": 1000
    },
    "Security": {
      "ValidateSslCertificates": true,
      "RequireHttps": true
    },
    "Export": {
      "DefaultExportPath": "Exports",
      "DefaultExportFormat": "JSON"
    }
  }
}
```

### 環境變數

您可以使用 `FHIR_` 前綴的環境變數覆蓋配置：

```bash
FHIR_QUERY_BUILDER_DEFAULT_SERVER_URL=https://my-fhir-server.com
FHIR_QUERY_BUILDER_REQUEST_TIMEOUT_SECONDS=60
```

## 使用方法

### 基本查詢建構

1. **連接伺服器**: 輸入 FHIR 伺服器 URL 並點擊「連接」
2. **選擇資源**: 從下拉選單選擇資源類型
3. **新增參數**: 使用參數建構器新增搜尋條件
4. **建構查詢**: 點擊「建立 URL」產生查詢
5. **執行**: 點擊「搜尋」執行查詢並檢視結果

### 🆕 進階搜尋使用指南

#### 使用進階搜尋頁籤

1. **切換到進階搜尋**: 點擊「Advanced」頁籤
2. **選擇功能類型**:
   - **Chaining**: 鏈式搜尋
   - **Composite**: 複合參數
   - **Filter**: 過濾表達式
   - **Result Control**: 結果控制

#### 鏈式搜尋範例

1. 在 Chaining 頁籤中：
   - **Path**: `patient.name`
   - **Value**: `John`
   - 點擊「Add」新增參數

2. 結果查詢: `Observation?patient.name=John`

#### 複合參數範例

1. 在 Composite 頁籤中：
   - **Parameter**: `component-code-value-quantity`
   - **Components**: `http://loinc.org|8480-6,120,mmHg`
   - 點擊「Add」新增參數

2. 結果查詢: `Observation?component-code-value-quantity=http://loinc.org|8480-6$120$mmHg`

#### 過濾表達式範例

1. 在 Filter 頁籤中：
   - **Expression**: `name co 'John' and birthDate ge 1990-01-01`
   - 點擊「Add」新增過濾條件

2. 結果查詢: `Patient?_filter=name co 'John' and birthDate ge 1990-01-01`

#### 結果控制範例

1. 在 Result Control 頁籤中設定：
   - **Count**: `50` (每頁結果數)
   - **Offset**: `0` (起始位置)
   - **Total**: `accurate` (精確總數)
   - **Summary**: `false` (完整結果)
   - **Elements**: `id,name,birthDate` (指定元素)

2. 結果查詢會自動包含這些控制參數

### 使用流暢 API

```csharp
// 基本查詢建構
var query = queryBuilder
    .ForResource("Patient")
    .WhereString("family", "Smith")
    .WhereDate("birthdate", DateTime.Parse("1990-01-01"), SearchPrefix.GreaterEqual)
    .WhereToken("gender", "male")
    .Include("Patient:organization")
    .Count(50)
    .BuildUrl("https://server.fire.ly");

// 複雜查詢範例
var complexQuery = queryBuilder
    .ForResource("Observation")
    .WhereToken("code", "8310-5", "http://loinc.org")
    .WhereReference("subject", "Patient/123")
    .WhereDate("date", DateTime.Now.AddDays(-30), SearchPrefix.GreaterEqual)
    .WhereNumber("value-quantity", 120, SearchPrefix.GreaterThan)
    .Include("Observation:subject")
    .RevInclude("DiagnosticReport:result")
    .Sort("date", SortOrder.Descending)
    .Count(100)
    .Summary("data")
    .BuildUrl("https://server.fire.ly");

// 🆕 進階搜尋功能範例
// 鏈式搜尋
var chainingQuery = queryBuilder
    .ForResource("Observation")
    .WhereString("status", "final")
    .Chain("patient.name", "John")
    .Chain("patient.organization.name", "General Hospital")
    .Count(50)
    .BuildUrl("https://server.fire.ly");

// 反向鏈式搜尋
var reverseChainQuery = queryBuilder
    .ForResource("Patient")
    .WhereString("active", "true")
    .ReverseChain("Observation", "patient", "code=8480-6")
    .Include("Patient:organization")
    .BuildUrl("https://server.fire.ly");

// 複合參數搜尋
var components = CompositeParameterExtensions.CreateTokenQuantityComponents(
    "http://loinc.org", "8480-6", 120, "mmHg");
var compositeQuery = queryBuilder
    .ForResource("Observation")
    .WhereString("status", "final")
    .WhereComposite("component-code-value-quantity", components)
    .Include("Observation:patient")
    .BuildUrl("https://server.fire.ly");

// 過濾表達式搜尋
var filterQuery = queryBuilder
    .ForResource("Patient")
    .Filter("name co 'John' and birthDate ge 1990-01-01")
    .Count(100)
    .Total("accurate")
    .Elements("id", "name", "birthDate", "active")
    .BuildUrl("https://server.fire.ly");

// 完整進階搜尋組合
var advancedQuery = queryBuilder
    .ForResource("Observation")
    .WhereString("category", "vital-signs")
    .Chain("patient.name", "John")
    .WhereComposite("component-code-value-quantity",
        "http://loinc.org|8480-6", "120", "mmHg")
    .Filter("effectiveDateTime ge 2023-01-01")
    .Include("Observation:patient")
    .RevInclude("DiagnosticReport:result")
    .Count(50)
    .Offset(0)
    .Sort("effectiveDateTime")
    .Total("accurate")
    .Summary("false")
    .Elements("id", "status", "code", "valueQuantity", "effectiveDateTime")
    .BuildUrl("https://server.fire.ly");

// 使用查詢驗證
if (queryBuilder.IsValid())
{
    var url = queryBuilder.BuildUrl("https://server.fire.ly");
    var result = await fhirQueryService.ExecuteQueryAsync(url);
}
else
{
    var errors = queryBuilder.GetValidationErrors();
    // 處理驗證錯誤
}
```

### 查詢範本

建立可重複使用的查詢範本：

1. 使用視覺化建構器建立查詢
2. 點擊「儲存為範本」
3. 提供名稱和描述
4. 從範本選單存取

### 匯出結果

以多種格式匯出查詢結果：

1. 執行查詢以取得結果
2. 點擊「匯出」按鈕
3. 選擇格式（JSON、XML、CSV）
4. 選擇匯出位置
5. 配置匯出選項

### 進階功能

#### 效能監控
應用程式提供即時效能監控：
- 查詢執行時間追蹤
- 記憶體使用監控
- 快取命中率統計
- 伺服器回應時間分析

#### 錯誤處理
全面的錯誤處理機制：
- 自動錯誤分類和報告
- 詳細的錯誤訊息和建議
- 錯誤歷史記錄和分析
- 自動重試機制

#### 查詢優化
智慧查詢優化功能：
- 查詢複雜度分析
- 效能建議
- 自動參數優化
- 結果快取策略

## 故障排除

### 常見問題

#### 連接問題
**問題**: 無法連接到 FHIR 伺服器
**解決方案**:
1. 檢查伺服器 URL 是否正確
2. 確認網路連接
3. 檢查防火牆設定
4. 驗證 SSL 憑證設定

#### 效能問題
**問題**: 查詢執行緩慢
**解決方案**:
1. 啟用快取功能
2. 調整連接池設定
3. 使用分頁載入大型結果
4. 優化查詢參數

#### 記憶體問題
**問題**: 應用程式記憶體使用過高
**解決方案**:
1. 調整快取大小限制
2. 啟用結果串流
3. 定期清理查詢歷史
4. 使用分頁處理大型資料集

### 日誌和診斷

啟用詳細日誌以進行診斷：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "FHIRQueryBuilder": "Debug"
    }
  }
}
```

## 架構設計

### 專案結構

```
FHIRQueryBuilder/
├── Configuration/          # 配置類別和提供者
├── Services/              # 業務邏輯服務
│   ├── Interfaces/        # 服務合約
│   ├── FhirQueryService.cs
│   ├── ValidationService.cs
│   ├── CacheService.cs
│   └── ...
├── QueryBuilders/         # 查詢建構元件
│   ├── FluentApi/         # 流暢 API 實作
│   ├── Interfaces/        # 建構器合約
│   └── Parameter builders
├── ViewModels/            # MVVM ViewModels
├── Models/                # 資料模型
└── Forms/                 # Windows Forms UI
```

### 核心服務

- **IFhirQueryService**: 核心 FHIR 操作和伺服器通訊
- **IValidationService**: 輸入驗證、URL 驗證和查詢複雜度檢查
- **ICacheService**: 記憶體快取和分散式快取抽象層
- **IPerformanceService**: 效能監控、指標收集和報告產生
- **IErrorHandlingService**: 錯誤追蹤、分類和事件通知
- **IProgressService**: 操作進度追蹤和即時更新
- **IExportService**: 多格式結果匯出（JSON、XML、CSV）
- **IQueryTemplateService**: 查詢範本管理和歷史記錄
- **IConfigurationService**: 配置管理、驗證和環境支援
- **IConnectionPoolService**: HTTP 連接池管理和優化
- **IPaginationService**: 結果分頁和大型資料集處理

### 設計模式

- **策略模式**: 不同 FHIR 類型的參數建構器
- **工廠模式**: 參數建構器工廠
- **觀察者模式**: 事件驅動更新
- **儲存庫模式**: 資料存取抽象
- **命令模式**: 查詢執行

## 測試

執行測試套件：

```bash
dotnet test
```

測試專案包括：
- 所有服務的單元測試
- 端到端情境的整合測試
- 關鍵路徑的效能測試
- 外部相依性的模擬實作

## 效能

### 優化功能

- **連接池**: 重複使用 HTTP 連接
- **智慧快取**: 快取能力聲明和查詢結果
- **非同步操作**: 支援取消的非阻塞 UI
- **分頁**: 有效處理大型結果集
- **壓縮**: 可選的回應壓縮
- **重試邏輯**: 指數退避的自動重試

### 監控

應用程式包含全面的效能監控：

- 查詢執行時間
- 記憶體使用追蹤
- 快取命中/未命中比率
- 錯誤率和模式
- 伺服器回應時間

## 安全性

### 功能

- **HTTPS 強制**: 可配置的 HTTPS 需求
- **憑證驗證**: SSL/TLS 憑證驗證
- **權杖加密**: 驗證權杖的安全儲存
- **輸入驗證**: 全面的輸入清理
- **稽核日誌**: 安全事件日誌

### 最佳實務

- 在生產環境中始終使用 HTTPS
- 驗證伺服器憑證
- 安全地儲存敏感配置
- 啟用稽核日誌
- 定期安全更新

## 貢獻

1. Fork 儲存庫
2. 建立功能分支
3. 進行變更
4. 為新功能新增測試
5. 確保所有測試通過
6. 提交 pull request

### 開發指南

- 遵循 C# 編碼慣例
- 撰寫全面的測試
- 記錄公開 API
- 對 I/O 操作使用 async/await
- 優雅地處理錯誤
- 記錄重要事件

## 授權

此專案採用 MIT 授權 - 詳情請參閱 LICENSE 檔案。

## 支援

如需支援和問題：

- 在 GitHub 上建立 issue
- 查看文件
- 檢閱現有的 issues 和討論

## 變更日誌

### 版本 1.0.0
- 具備核心功能的初始版本
- 視覺化查詢建構器
- FHIR 伺服器連接
- 基本匯出功能

### 版本 1.1.0
- 新增流暢 API
- 效能監控
- 進階快取
- 查詢範本
- 改進的錯誤處理

## 技術規格

### 平台需求
- **作業系統**: Windows 10 版本 1809 或更新版本、Windows 11、Windows Server 2019 或更新版本
- **.NET 版本**: .NET 8.0 Runtime（自包含部署中已包含）
- **記憶體**: 最少 512 MB RAM，建議 2 GB 或更多
- **磁碟空間**: 最少 100 MB 可用空間

### 支援的 FHIR 版本
- **FHIR R4** (4.0.1) - 完全支援
- **FHIR R5** (5.0.0) - 完全支援
- **FHIR STU3** - 有限支援

### 技術堆疊
- **.NET 8.0**: 最新的 .NET 平台
- **Windows Forms**: 原生 Windows UI 框架
- **Microsoft.Extensions.DependencyInjection**: 依賴注入容器
- **CommunityToolkit.Mvvm**: MVVM 框架支援
- **Microsoft.Extensions.Configuration**: 配置管理
- **Microsoft.Extensions.Logging**: 結構化日誌
- **System.Text.Json**: JSON 序列化
- **xUnit + Moq + FluentAssertions**: 測試框架

## 部署選項

### 1. 獨立執行檔部署
```bash
# 建置自包含的單一檔案
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### 2. Docker 容器部署
```bash
# 建置 Docker 映像
docker build -t fhir-query-builder .

# 執行容器
docker run -d -p 8080:80 fhir-query-builder
```

### 3. 使用部署腳本
```powershell
# 執行自動化部署腳本
.\Scripts\deploy.ps1 -Configuration Release -CreateInstaller
```

## 發展藍圖

- [ ] 額外的匯出格式（Excel、PDF）
- [ ] 進階查詢驗證和語法檢查
- [ ] FHIR 批次操作支援
- [ ] 外掛架構和擴展點
- [ ] 網頁版介面
- [ ] 多伺服器管理和比較
- [ ] GraphQL 查詢支援
- [ ] 即時查詢結果串流
- [ ] 進階資料視覺化
- [ ] 自動化測試產生
