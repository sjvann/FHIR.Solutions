# FHIR Primitive Type 設計架構

## 概述

基於 [FHIR R5 Datatypes](https://hl7.org/fhir/R5/datatypes.html#primitive) 規範，本架構提供完整的 FHIR Primitive Type 實作，解決命名衝突問題並支援 FHIR 的 JSON 序列化格式。

## 核心設計原則

### 1. **命名規範**
- 所有 FHIR Primitive Type 加上 `Fhir` 前綴
- 避免與程式語言型別名稱衝突
- 清楚區分模型層級與實作層級

### 2. **JSON 序列化支援**
- 支援 FHIR R5 的兩種 JSON 表示方式
- 簡化表示：直接使用值
- 完整表示：包含 Extension 和 ID

### 3. **NULL 值處理**
- 統一的 NULL 檢核機制
- 支援 FHIR 的 NULL 語義

## FHIR Primitive Types 對應

| FHIR Primitive Type | C# 型別 | 我們的實作 | 說明 |
|-------------------|---------|-----------|------|
| base64Binary | byte[] | FhirBase64Binary | Base64 編碼的二進位資料 |
| boolean | bool | FhirBoolean | 布林值 |
| canonical | string | FhirCanonical | 規範化 URL |
| code | string | FhirCode | 程式碼值 |
| date | DateTime | FhirDate | 日期 |
| dateTime | DateTime | FhirDateTime | 日期時間 |
| decimal | decimal | FhirDecimal | 十進位數 |
| id | string | FhirId | 資源 ID |
| instant | DateTime | FhirInstant | 瞬間時間 |
| integer | int | FhirInteger | 整數 |
| integer64 | long | FhirInteger64 | 64 位元整數 |
| markdown | string | FhirMarkdown | Markdown 格式文字 |
| oid | string | FhirOid | OID |
| positiveInt | uint | FhirPositiveInt | 正整數 |
| string | string | FhirString | 字串 |
| time | TimeSpan | FhirTime | 時間 |
| unsignedInt | uint | FhirUnsignedInt | 無符號整數 |
| uri | string | FhirUri | URI |
| url | string | FhirUrl | URL |
| uuid | Guid | FhirUuid | UUID |

## JSON 序列化格式

### 1. **簡化表示**
```json
{
  "count": 42,
  "name": "John Doe",
  "active": true
}
```

### 2. **完整表示**
```json
{
  "count": 42,
  "_count": {
    "id": "a1",
    "extension": [
      {
        "url": "http://example.com/extension",
        "valueString": "additional info"
      }
    ]
  }
}
```

## 架構設計

### 1. **介面層 (Abstractions)**

```csharp
public interface IPrimitiveType<TValue> : ITypeFramework
    where TValue : IConvertible
{
    TValue? Value { get; set; }
    string? StringValue { get; set; }
    bool IsNull { get; }
    
    // 抽象方法
    TValue? ParseValue(string value);
    string? ValueToString(TValue? value);
    bool IsValidValue(TValue? value);
    
    // JSON 序列化
    JsonValue? ToJsonValue();
    void FromJsonValue(JsonValue? jsonValue);
    JsonObject? ToFullJsonObject();
    void FromFullJsonObject(JsonObject? jsonObject);
}
```

### 2. **基礎類別層 (Base)**

```csharp
public abstract class PrimitiveTypeBase<TValue> : PrimitiveType, IPrimitiveType<TValue>
    where TValue : IConvertible
{
    // 通用實作
    public TValue? Value { get; set; }
    public string? StringValue { get; set; }
    public bool IsNull => Value == null;
    
    // 抽象方法 - 子類別必須實作
    public abstract TValue? ParseValue(string value);
    public abstract string? ValueToString(TValue? value);
    public abstract bool IsValidValue(TValue? value);
    
    // JSON 序列化實作
    public virtual JsonValue? ToJsonValue() { /* 實作 */ }
    public virtual void FromJsonValue(JsonValue? jsonValue) { /* 實作 */ }
    public virtual JsonObject? ToFullJsonObject() { /* 實作 */ }
    public virtual void FromFullJsonObject(JsonObject? jsonObject) { /* 實作 */ }
}
```

### 3. **實作層 (DataTypes)**

```csharp
public class FhirString : PrimitiveTypeBase<string>
{
    // 只需要實作三個抽象方法
    public override string? ParseValue(string value) => value;
    public override string? ValueToString(string? value) => value;
    public override bool IsValidValue(string? value) => /* 驗證邏輯 */;
    
    // 隱含轉換
    public static implicit operator FhirString?(string? value) => 
        value == null ? null : new FhirString(value);
    public static implicit operator string?(FhirString? fhirString) => 
        fhirString?.Value;
}
```

## 使用範例

### 1. **基本使用**

```csharp
// 建立實例
var fhirString = new FhirString("Hello World");
var fhirInteger = new FhirInteger(42);
var fhirBoolean = new FhirBoolean(true);

// 隱含轉換
FhirString str = "Hello";  // 自動轉換
string value = str;         // 自動轉換
```

### 2. **JSON 序列化**

```csharp
var serializer = new FhirJsonSerializer();

// 簡化序列化
var simpleJson = serializer.SerializeSimple(fhirString);
// 結果: "Hello World"

// 完整序列化
var fullJson = serializer.SerializeFull(fhirString);
// 結果: { "value": "Hello World", "id": "a1", "extension": [...] }

// FHIR 格式序列化
var fhirJson = serializer.SerializeFhirFormat("name", fhirString);
// 結果: { "name": "Hello World", "_name": { "id": "a1", "extension": [...] } }
```

### 3. **驗證使用**

```csharp
// 驗證物件
var validationContext = new ValidationContext(fhirString);
var results = fhirString.Validate(validationContext);

foreach (var result in results)
{
    Console.WriteLine($"Validation Error: {result.ErrorMessage}");
}
```

### 4. **Extension 支援**

```csharp
// 添加 Extension
fhirString.AddExtension("http://example.com/custom", "additional info");

// 取得 Extension
var ext = fhirString.GetExtension("http://example.com/custom");

// 移除 Extension
fhirString.RemoveExtension("http://example.com/custom");
```

## 優點

### 1. **命名清晰**
- 避免與程式語言型別衝突
- 清楚區分 FHIR 模型與程式語言型別

### 2. **完整 FHIR 支援**
- 支援 FHIR R5 的所有 Primitive Types
- 完整的 JSON 序列化格式支援
- Extension 功能支援

### 3. **型別安全**
- 強型別設計
- 編譯時期檢查
- 隱含轉換支援

### 4. **開發者體驗**
- 簡化的 API
- 豐富的 IntelliSense 支援
- 完整的文件註解

### 5. **可擴展性**
- 新增 Primitive Type 只需繼承 `PrimitiveTypeBase<T>`
- 實作三個抽象方法即可
- 統一的架構模式

## 未來擴展

### 1. **更多 Primitive Types**
```csharp
public class FhirBase64Binary : PrimitiveTypeBase<byte[]> { }
public class FhirCanonical : PrimitiveTypeBase<string> { }
public class FhirCode : PrimitiveTypeBase<string> { }
public class FhirDate : PrimitiveTypeBase<DateTime> { }
public class FhirDateTime : PrimitiveTypeBase<DateTime> { }
public class FhirDecimal : PrimitiveTypeBase<decimal> { }
public class FhirId : PrimitiveTypeBase<string> { }
public class FhirInstant : PrimitiveTypeBase<DateTime> { }
public class FhirInteger64 : PrimitiveTypeBase<long> { }
public class FhirMarkdown : PrimitiveTypeBase<string> { }
public class FhirOid : PrimitiveTypeBase<string> { }
public class FhirPositiveInt : PrimitiveTypeBase<uint> { }
public class FhirTime : PrimitiveTypeBase<TimeSpan> { }
public class FhirUnsignedInt : PrimitiveTypeBase<uint> { }
public class FhirUri : PrimitiveTypeBase<string> { }
public class FhirUrl : PrimitiveTypeBase<string> { }
public class FhirUuid : PrimitiveTypeBase<Guid> { }
```

### 2. **自訂驗證規則**
```csharp
public class FhirEmail : PrimitiveTypeBase<string>
{
    public override bool IsValidValue(string? value)
    {
        return !string.IsNullOrEmpty(value) && 
               System.Text.RegularExpressions.Regex.IsMatch(value, @"^[^@]+@[^@]+\.[^@]+$");
    }
}
```

### 3. **效能優化**
- 快取解析結果
- 物件池 (Object Pooling)
- 記憶體優化

## 結論

這個設計完全符合 FHIR R5 規範，解決了命名衝突問題，並提供了完整的 JSON 序列化支援。架構清晰、易於使用，為 FHIR SDK 提供了堅實的基礎。 