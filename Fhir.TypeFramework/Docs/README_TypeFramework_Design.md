# FHIR Type Framework 設計架構

## 概述

本文件描述 FHIR Type Framework 的設計架構，基於 FHIR R5 規範，為 .NET C# 開發者提供一個完整的 FHIR SDK 架構。

## 核心設計原則

### 1. 介面導向設計 (Interface-First Design)

使用介面來定義所有 FHIR 型別的基本行為，確保：
- **可測試性**：可以輕鬆模擬 (Mock) 各種型別
- **可擴展性**：新功能可以透過介面擴展
- **鬆耦合**：實作與介面分離

### 2. 解決循環依賴問題

原始問題：
```csharp
// 問題：循環依賴
public abstract class Element : Base
{
    public List<Extension>? Extension { get; set; }  // Extension 繼承自 Element
}

public class Extension : Element  // 形成循環依賴
{
    public string? Url { get; set; }
}
```

解決方案：
```csharp
// 解決：使用介面分離
public interface IExtension : ITypeFramework
{
    string? Url { get; set; }
    object? Value { get; set; }
}

public class Extension : Base, IExtension
{
    public string? Url { get; set; }
    public object? Value { get; set; }
}

public abstract class Element : Base, IExtensibleTypeFramework
{
    public IList<IExtension>? Extension { get; set; }  // 使用介面而非具體型別
}
```

## 架構層次

### 1. 核心介面層 (Abstractions)

```
ITypeFramework (核心介面)
├── ISerializableTypeFramework (可序列化)
├── IIdentifiableTypeFramework (具有 ID)
├── IExtensibleTypeFramework (具有擴展)
└── IExtension (擴展介面)
```

### 2. 基礎類別層 (Base)

```
Base (所有型別的根)
├── Element (資源內元素的基礎)
│   └── BackboneElement (骨幹元素)
├── DataType (資料型別基礎)
│   └── PrimitiveType (基本型別基礎)
└── Resource (資源基礎)
```

### 3. 資料型別層 (DataTypes)

```
Extension (擴展型別)
├── PrimitiveTypes/ (基本型別)
│   ├── String
│   ├── Integer
│   ├── Boolean
│   └── ...
└── ComplexTypes/ (複雜型別)
    ├── CodeableConcept
    ├── Reference
    └── ...
```

## 設計模式

### 1. 工廠模式 (Factory Pattern)

```csharp
public interface ITypeFrameworkFactory
{
    T Create<T>() where T : ITypeFramework, new();
    ITypeFramework? Create(string typeName);
    IExtension CreateExtension(string url, object? value);
}
```

### 2. 註冊器模式 (Registry Pattern)

```csharp
public interface ITypeRegistry
{
    void Register<T>() where T : ITypeFramework, new();
    Type? GetType(string typeName);
    IEnumerable<string> GetRegisteredTypeNames();
}
```

### 3. 驗證模式 (Validation Pattern)

```csharp
public interface ITypeFramework
{
    IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
}
```

## 使用範例

### 1. 基本使用

```csharp
// 建立工廠
var registry = new TypeRegistry();
var factory = new TypeFrameworkFactory(registry);

// 註冊型別
registry.Register<Extension>();

// 建立實例
var extension = factory.CreateExtension("http://example.com/extension", "value");
var element = factory.Create<MyElement>();
```

### 2. 擴展使用

```csharp
// 添加擴展
element.AddExtension("http://example.com/custom", "customValue");

// 取得擴展
var ext = element.GetExtension("http://example.com/custom");

// 移除擴展
element.RemoveExtension("http://example.com/custom");
```

### 3. 驗證使用

```csharp
// 驗證物件
var validationContext = new ValidationContext(element);
var results = element.Validate(validationContext);

foreach (var result in results)
{
    Console.WriteLine($"Validation Error: {result.ErrorMessage}");
}
```

## 優點

### 1. 型別安全
- 強型別設計，編譯時期檢查
- 避免執行時期錯誤

### 2. 可擴展性
- 新型別可以輕鬆加入
- 現有型別可以擴展功能

### 3. 可測試性
- 介面導向設計便於單元測試
- 可以輕鬆模擬各種情境

### 4. 效能優化
- 避免不必要的物件建立
- 使用適當的資料結構

### 5. 開發者體驗
- 直觀的 API 設計
- 豐富的 IntelliSense 支援
- 完整的文件註解

## 未來擴展

### 1. 序列化支援
- JSON 序列化/反序列化
- XML 序列化/反序列化

### 2. 驗證增強
- 自訂驗證規則
- 跨欄位驗證

### 3. 效能優化
- 物件池 (Object Pooling)
- 快取機制

### 4. 工具支援
- 程式碼產生器
- 視覺化設計工具

## 結論

這個架構設計解決了 FHIR Type Framework 中的循環依賴問題，同時提供了：
- 清晰的型別層次結構
- 靈活的擴展機制
- 優秀的開發者體驗
- 良好的可維護性

這為建立一個完整的 FHIR SDK 奠定了堅實的基礎。 