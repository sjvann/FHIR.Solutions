# FHIR R5 Type Framework 實作文件

## 📋 專案概述

本專案實作了完整的 FHIR R5 Type Framework，提供強型別、安全且符合 FHIR 規範的 .NET 開發框架。

## 🏗️ 架構設計

### 核心設計原則

1. **強型別安全**：所有 FHIR 型別都有對應的強型別 C# 類別
2. **FHIR 規範遵循**：完全符合 FHIR R5 規範
3. **擴展性**：支援 FHIR 擴展機制
4. **驗證機制**：內建 FHIR 規範驗證
5. **深層複製**：支援物件的深層複製
6. **相等性比較**：正確的物件相等性比較

### 類別層次結構

```
Base (基礎類別) ← 對應 FHIR R5 的 BaseElement
├── Element (元素)
    ├── DataType (資料型別)
    │   ├── PrimitiveType (原始類型)
    │   │   ├── FhirString, FhirId, FhirUri, FhirCode, FhirBoolean, etc.
    │   │   └── 所有 FHIR Primitive Types
    │   ├── Resource (資源)
    │   │   └── DomainResource (領域資源)
    │   │       ├── CanonicalResource (規範資源)
    │   │       └── MetadataResource (元資料資源)
    │   └── BackboneType (骨幹型別)
    └── BackboneElement (骨幹元素)
```

## 📁 專案結構

### Base 目錄 - 基礎類別

| 檔案 | 描述 | FHIR R5 對應 |
|------|------|---------------|
| `Base.cs` | 所有 FHIR 型別的最基礎類別 | BaseElement |
| `Element.cs` | 元素基礎類別，包含 id 和 extension | Element |
| `DataType.cs` | 資料型別基礎類別 | DataType |
| `PrimitiveType.cs` | 原始型別基礎類別 | PrimitiveType |
| `Resource.cs` | 資源基礎類別 | Resource |
| `DomainResource.cs` | 領域資源基礎類別 | DomainResource |
| `CanonicalResource.cs` | 規範資源基礎類別 | CanonicalResource |
| `MetadataResource.cs` | 元資料資源基礎類別 | MetadataResource |
| `BackboneElement.cs` | 骨幹元素基礎類別 | BackboneElement |
| `BackboneType.cs` | 骨幹型別基礎類別 | BackboneType |

### DataTypes 目錄 - 資料型別

#### PrimitiveTypes
- `FhirString.cs` - 字串型別
- `FhirId.cs` - 識別碼型別
- `FhirUri.cs` - URI 型別
- `FhirCode.cs` - 代碼型別
- `FhirBoolean.cs` - 布林型別
- `FhirInteger.cs` - 整數型別
- `FhirDecimal.cs` - 小數型別
- `FhirDateTime.cs` - 日期時間型別
- `FhirDate.cs` - 日期型別
- `FhirTime.cs` - 時間型別
- `FhirMarkdown.cs` - Markdown 型別
- `FhirUrl.cs` - URL 型別
- `FhirCanonical.cs` - 規範型別

#### ComplexTypes
- `Extension.cs` - 擴展型別
- `Annotation.cs` - 註解型別
- `Reference.cs` - 參考型別
- `Quantity.cs` - 數量型別
- `CodeableConcept.cs` - 可編碼概念型別
- `Period.cs` - 期間型別
- `Timing.cs` - 時程型別
- `Age.cs` - 年齡型別
- `Range.cs` - 範圍型別
- `Duration.cs` - 持續時間型別
- `Meta.cs` - 元資料型別
- `Count.cs` - 計數型別
- `Distance.cs` - 距離型別
- `SimpleQuantity.cs` - 簡單數量型別
- `SampledData.cs` - 採樣資料型別
- `Narrative.cs` - 敘述型別
- `ContactPoint.cs` - 聯絡點型別
- `ContactDetail.cs` - 聯絡詳情型別
- `UsageContext.cs` - 使用上下文型別
- `RelatedArtifact.cs` - 相關文件型別

### Abstractions 目錄 - 介面定義

| 檔案 | 描述 |
|------|------|
| `ITypeFramework.cs` | 型別框架基礎介面 |
| `IPrimitiveType.cs` | 原始型別介面 |
| `IResource.cs` | 資源介面 |
| `IDomainResource.cs` | 領域資源介面 |
| `ICanonicalResource.cs` | 規範資源介面 |
| `IMetadataResource.cs` | 元資料資源介面 |
| `IIdentifiableTypeFramework.cs` | 可識別型別介面 |
| `IExtensibleTypeFramework.cs` | 可擴展型別介面 |

### Factories 目錄 - 工廠模式

| 檔案 | 描述 |
|------|------|
| `ITypeFrameworkFactory.cs` | 型別框架工廠介面 |
| `TypeFrameworkFactory.cs` | 型別框架工廠實作 |

### Serialization 目錄 - 序列化

| 檔案 | 描述 |
|------|------|
| `FhirJsonSerializer.cs` | FHIR JSON 序列化器 |

### Examples 目錄 - 使用範例

| 檔案 | 描述 |
|------|------|
| `ChoiceTypeUsageExample.cs` | Choice Type 使用範例 |
| `ResourceHierarchyExample.cs` | 資源層次結構範例 |

## 🔧 核心功能

### 1. FHIR Primitive Types

所有 FHIR Primitive Types 都使用 `Fhir` 前綴，避免與 C# 原生型別衝突：

```csharp
// 正確的 FHIR Primitive Types
public FhirString? Name { get; set; }
public FhirId? Id { get; set; }
public FhirUri? Url { get; set; }
public FhirCode? Status { get; set; }
public FhirBoolean? Active { get; set; }
```

### 2. Choice Types ([x] 屬性)

使用強型別的 Choice Types 實作：

```csharp
// 定義特定的 Choice Type
public class ExtensionValueChoice : ChoiceType<FhirString, FhirInteger, FhirBoolean, FhirDecimal, FhirDateTime>
{
    // 強型別的方法
    public void SetStringValue(FhirString value) => SetValue(value);
    public void SetIntegerValue(FhirInteger value) => SetValue(value);
    public void SetBooleanValue(FhirBoolean value) => SetValue(value);
    public void SetDecimalValue(FhirDecimal value) => SetValue(value);
    public void SetDateTimeValue(FhirDateTime value) => SetValue(value);
}
```

### 3. 擴展機制

完整的 FHIR 擴展支援：

```csharp
// 添加擴展
element.AddExtension("http://example.com/extension", new FhirString("value"));

// 取得擴展
var extension = element.GetExtension("http://example.com/extension");

// 移除擴展
element.RemoveExtension("http://example.com/extension");
```

### 4. 驗證機制

內建的 FHIR 規範驗證：

```csharp
// 驗證資源
var validationResults = resource.Validate(new ValidationContext(resource));
foreach (var result in validationResults)
{
    Console.WriteLine($"驗證錯誤: {result.ErrorMessage}");
}
```

### 5. 深層複製

支援物件的深層複製：

```csharp
// 深層複製資源
var copy = resource.DeepCopy() as Patient;
```

### 6. 相等性比較

正確的物件相等性比較：

```csharp
// 比較兩個資源是否相等
bool areEqual = resource1.IsExactly(resource2);
```

## 🚀 使用範例

### 基本使用

```csharp
// 建立一個簡單的資源
var patient = new Patient
{
    Id = "patient-123",
    Name = new HumanName
    {
        Family = "張",
        Given = new List<FhirString> { "三" }
    },
    BirthDate = new FhirDate("1990-01-01"),
    Gender = new FhirCode("male")
};

// 添加擴展
patient.AddExtension("http://example.com/custom", new FhirString("custom-value"));

// 驗證資源
var validationResults = patient.Validate(new ValidationContext(patient));
```

### 資源層次結構

```csharp
// 建立規範資源
var canonicalResource = new ExampleCanonicalResource
{
    Url = "http://example.com/resource",
    Version = "1.0.0",
    Status = "active",
    Publisher = "Example Publisher"
};

// 建立元資料資源
var metadataResource = new ExampleMetadataResource
{
    ApprovalDate = new FhirDate("2024-01-01"),
    LastReviewDate = new FhirDate("2024-06-01"),
    Publisher = "Example Publisher"
};
```

## 📊 符合性檢查

### ✅ FHIR R5 規範符合性

- [x] 正確的類別層次結構
- [x] 正確的屬性定義和基數
- [x] 正確的 FHIR Path 映射
- [x] 正確的資料型別使用
- [x] 正確的擴展機制
- [x] 正確的驗證邏輯
- [x] 正確的序列化支援

### ✅ 技術特性

- [x] 強型別安全
- [x] 完整的 IntelliSense 支援
- [x] 編譯時錯誤檢查
- [x] 執行時驗證
- [x] 深層複製支援
- [x] 相等性比較
- [x] JSON 序列化
- [x] 擴展機制

## 🔄 版本歷史

### v1.0.0 (2024-12-19)
- ✅ 實作完整的 FHIR R5 Type Framework
- ✅ 建立正確的類別層次結構
- ✅ 實作所有 FHIR Primitive Types
- ✅ 實作所有 FHIR Complex Types
- ✅ 實作 Choice Types 機制
- ✅ 實作擴展機制
- ✅ 實作驗證機制
- ✅ 實作深層複製
- ✅ 實作相等性比較
- ✅ 提供完整的使用範例

## 📝 開發指南

### 添加新的 FHIR Primitive Type

1. 在 `DataTypes/PrimitiveTypes/` 目錄下創建新檔案
2. 繼承自 `PrimitiveTypeBase`
3. 實作必要的抽象方法
4. 添加適當的驗證邏輯

### 添加新的 FHIR Complex Type

1. 在 `DataTypes/ComplexTypes/` 目錄下創建新檔案
2. 繼承自 `DataType`
3. 定義屬性和驗證邏輯
4. 實作 `DeepCopy`、`IsExactly` 和 `Validate` 方法

### 添加新的 Choice Type

1. 在 `DataTypes/ComplexTypes/` 目錄下創建新檔案
2. 繼承自 `ChoiceType<T1, T2, ...>`
3. 定義強型別的方法
4. 添加適當的驗證邏輯

## 🤝 貢獻指南

1. Fork 專案
2. 創建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交變更 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 開啟 Pull Request

## 📄 授權

本專案採用 MIT 授權條款 - 詳見 [LICENSE](LICENSE) 檔案

## 📞 聯絡資訊

如有任何問題或建議，請開啟 Issue 或聯絡開發團隊。

---

**注意**：本文件會隨著專案的發展持續更新。 