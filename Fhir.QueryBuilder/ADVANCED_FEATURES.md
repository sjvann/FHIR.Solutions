# FHIR Query Builder - 進階功能指南

本文件詳細介紹 FHIR Query Builder v2.0 中新增的進階搜尋功能。

## 📋 目錄

- [鏈式搜尋 (Chaining Search)](#鏈式搜尋-chaining-search)
- [反向鏈式搜尋 (Reverse Chaining)](#反向鏈式搜尋-reverse-chaining)
- [複合參數 (Composite Parameters)](#複合參數-composite-parameters)
- [過濾表達式 (Filter Expressions)](#過濾表達式-filter-expressions)
- [結果控制參數](#結果控制參數)
- [實際醫療場景範例](#實際醫療場景範例)

## 🔗 鏈式搜尋 (Chaining Search)

鏈式搜尋允許您跨相關資源進行搜尋，通過參考關係連接不同的資源。

### 語法
```
resourceType?reference.searchParam=value
```

### UI 使用方法
1. 切換到「Advanced」頁籤
2. 選擇「Chaining」子頁籤
3. 輸入 Path 和 Value
4. 點擊「Add」新增參數

### 程式化使用
```csharp
var query = builder
    .ForResource("Observation")
    .Chain("patient.name", "John")
    .Chain("patient.organization.name", "General Hospital")
    .BuildQueryString();
```

### 實用範例
- `Observation?patient.name=John` - 搜尋名為 John 的患者的觀察記錄
- `DiagnosticReport?subject.organization.name=Hospital` - 搜尋特定醫院患者的診斷報告
- `MedicationRequest?patient.address-city=Taipei` - 搜尋台北市患者的用藥記錄

## 🔄 反向鏈式搜尋 (Reverse Chaining)

反向鏈式搜尋用於尋找被其他資源參考且符合特定條件的資源。

### 語法
```
resourceType?_has:targetResourceType:searchParam:value
```

### UI 使用方法
1. 在進階搜尋中選擇反向鏈式功能
2. 輸入目標資源類型、搜尋參數和值
3. 系統自動建構 `_has` 參數

### 程式化使用
```csharp
var query = builder
    .ForResource("Patient")
    .ReverseChain("Observation", "patient", "code=8480-6")
    .BuildQueryString();
```

### 實用範例
- `Patient?_has:Observation:patient:code=8480-6` - 有血壓檢測記錄的患者
- `Patient?_has:MedicationRequest:patient:medication=aspirin` - 有阿司匹林處方的患者
- `Organization?_has:Patient:organization:active=true` - 有活躍患者的機構

## 🧩 複合參數 (Composite Parameters)

複合參數允許您在單一參數中組合多個搜尋條件，用 `$` 分隔不同組件。

### 語法
```
parameterName=component1$component2$component3
```

### UI 使用方法
1. 選擇「Composite」頁籤
2. 輸入參數名稱
3. 輸入組件（用逗號分隔）
4. 系統自動轉換為 `$` 分隔格式

### 程式化使用
```csharp
// 使用擴展方法建立組件
var components = CompositeParameterExtensions.CreateTokenQuantityComponents(
    "http://loinc.org", "8480-6", 120, "mmHg");

var query = builder
    .ForResource("Observation")
    .WhereComposite("component-code-value-quantity", components)
    .BuildQueryString();
```

### 常用組合類型
1. **Token + Quantity**: 代碼 + 數值
   ```csharp
   CreateTokenQuantityComponents("http://loinc.org", "8480-6", 120, "mmHg")
   // 結果: http://loinc.org|8480-6$120$mmHg
   ```

2. **Token + Date**: 代碼 + 日期
   ```csharp
   CreateTokenDateComponents("http://loinc.org", "33747-0", DateTime.Now)
   // 結果: http://loinc.org|33747-0$2023-12-01
   ```

3. **Reference + Token**: 參考 + 代碼
   ```csharp
   CreateReferenceTokenComponents("Patient/123", "http://loinc.org", "8480-6")
   // 結果: Patient/123$http://loinc.org|8480-6
   ```

## 🔍 過濾表達式 (Filter Expressions)

過濾表達式使用 FHIR Filter 語法進行複雜的條件組合。

### 語法
```
_filter=expression
```

### 支援的操作符
- `eq` (等於), `ne` (不等於)
- `gt` (大於), `lt` (小於), `ge` (大於等於), `le` (小於等於)
- `co` (包含), `sw` (開始於), `ew` (結束於)
- `and`, `or`, `not` (邏輯操作符)

### UI 使用方法
1. 選擇「Filter」頁籤
2. 輸入完整的過濾表達式
3. 點擊「Add」新增過濾條件

### 程式化使用
```csharp
// 使用擴展方法建立過濾條件
var nameFilter = FilterParameterExtensions.CreateContainsFilter("name.family", "Smith");
var ageFilter = FilterParameterExtensions.CreateRangeFilter("birthDate", "1980-01-01", "2000-12-31");
var combinedFilter = FilterParameterExtensions.CreateAndFilter(nameFilter, ageFilter);

var query = builder
    .ForResource("Patient")
    .Filter(combinedFilter)
    .BuildQueryString();
```

### 實用範例
- `name co 'John' and birthDate ge 1990-01-01` - 名字包含 John 且 1990 年後出生
- `active eq true and gender eq 'male'` - 活躍的男性患者
- `(name co 'Smith' or name co 'Johnson') and birthDate le 2000-12-31` - 複雜條件組合

## ⚙️ 結果控制參數

控制查詢結果的格式、數量和內容。

### 可用參數

#### _count (每頁結果數)
```csharp
builder.Count(50)  // 每頁 50 筆結果
```

#### _offset (分頁偏移)
```csharp
builder.Offset(100)  // 從第 100 筆開始
```

#### _total (總數控制)
```csharp
builder.Total("accurate")  // none, estimate, accurate
```

#### _summary (摘要模式)
```csharp
builder.Summary("true")  // true, false, text, data, count
```

#### _elements (元素選擇)
```csharp
builder.Elements("id", "name", "birthDate")  // 只包含指定元素
```

## 🏥 實際醫療場景範例

### 場景 1：糖尿病患者血糖監測
```csharp
var diabetesMonitoring = builder
    .ForResource("Observation")
    .WhereToken("code", "33747-0", "http://loinc.org")  // 血糖檢測
    .Chain("patient.condition.code", "E11")  // 糖尿病患者
    .WhereDate("effectiveDateTime", "2023-01-01", SearchPrefix.GreaterEqual)
    .Filter("valueQuantity.value gt 126")  // 血糖值 > 126 mg/dL
    .Include("Observation:patient")
    .Count(100)
    .Sort("-effectiveDateTime")
    .BuildQueryString();
```

### 場景 2：藥物相互作用檢查
```csharp
var drugInteractionCheck = builder
    .ForResource("MedicationRequest")
    .WhereString("status", "active")
    .Filter("medicationCodeableConcept.coding.code eq '11289' or medicationCodeableConcept.coding.code eq '1191'")
    .Chain("patient.age", "65", SearchPrefix.GreaterEqual)
    .Include("MedicationRequest:patient")
    .Count(50)
    .BuildQueryString();
```

### 場景 3：品質指標監控
```csharp
var qualityMetrics = builder
    .ForResource("Observation")
    .WhereToken("code", "4548-4", "http://loinc.org")  // HbA1c
    .Chain("patient.condition.code", "E11")  // 糖尿病患者
    .Filter("effectiveDateTime ge 2023-01-01 and valueQuantity.value ge 7 and valueQuantity.value le 10")
    .Include("Observation:patient")
    .RevInclude("CarePlan:patient")
    .Count(200)
    .Total("accurate")
    .Summary("data")
    .BuildQueryString();
```

## 🔧 最佳實踐

### 1. 效能優化
- 使用適當的 `_count` 限制結果數量
- 優先使用索引欄位進行搜尋
- 避免過於複雜的過濾表達式

### 2. 錯誤處理
- 始終檢查查詢驗證結果
- 處理伺服器不支援的參數
- 提供使用者友善的錯誤訊息

### 3. 安全考量
- 驗證使用者輸入
- 避免 SQL 注入式攻擊
- 限制查詢複雜度

### 4. 可維護性
- 使用有意義的變數名稱
- 將複雜查詢分解為多個步驟
- 建立可重用的查詢範本

## 📚 參考資源

- [FHIR R4 Search Specification](https://hl7.org/fhir/R4/search.html)
- [FHIR R5 Search Specification](https://hl7.org/fhir/R5/search.html)
- [FHIR Filter Parameter](https://hl7.org/fhir/R4/search_filter.html)
- [FHIR Composite Parameters](https://hl7.org/fhir/R4/search.html#composite)

## 🤝 貢獻

如果您發現問題或有改進建議，請：
1. 建立 Issue 描述問題
2. 提交 Pull Request 包含修正
3. 更新相關文件

## 📞 支援

如需技術支援，請聯繫：
- GitHub Issues: [FhirServerHelper Issues](https://github.com/sjvann/FhirServerHelper/issues)
- Email: sjvann@gmail.com
