# FHIR 基本型別重構說明

## 問題分析

### 🔴 **原始問題**

1. **編譯錯誤**：
   - `PrimitiveTypeImp` 類別使用了不存在的 `ElementImp`
   - 缺少 `ToTitleCase`、`GetJsonNode` 等擴展方法
   - 泛型約束混亂且不一致

2. **架構問題**：
   - 與新的 `PrimitiveType` 架構衝突
   - 使用了舊的 `Element` 而非新的介面架構
   - 型別參數命名混亂（T1, T2）

3. **設計問題**：
   - 過度複雜的泛型設計
   - 缺乏清晰的職責分離
   - 驗證邏輯分散

## 解決方案

### 🟢 **新的架構設計**

```
PrimitiveType (抽象基礎類別)
└── PrimitiveTypeBase<TValue> (泛型基礎類別)
    ├── FhirString : PrimitiveTypeBase<string>
    ├── FhirInteger : PrimitiveTypeBase<int>
    ├── FhirBoolean : PrimitiveTypeBase<bool>
    └── ...
```

### 🎯 **核心改善**

#### 1. **清晰的職責分離**

```csharp
public abstract class PrimitiveTypeBase<TValue> : PrimitiveType
    where TValue : IConvertible
{
    // 值管理
    public TValue? Value { get; set; }
    public string? StringValue { get; set; }
    
    // 抽象方法 - 子類別必須實作
    protected abstract TValue? ParseValue(string value);
    protected abstract string? ValueToString(TValue? value);
    protected abstract bool IsValidValue(TValue? value);
    
    // 通用實作
    public override Base DeepCopy() { /* 實作 */ }
    public override bool IsExactly(Base other) { /* 實作 */ }
    public override IEnumerable<ValidationResult> Validate(ValidationContext context) { /* 實作 */ }
}
```

#### 2. **簡化的子類別實作**

```csharp
public class FhirString : PrimitiveTypeBase<string>
{
    // 只需要實作三個抽象方法
    protected override string? ParseValue(string value) => value;
    protected override string? ValueToString(string? value) => value;
    protected override bool IsValidValue(string? value) => /* 驗證邏輯 */;
    
    // 可選：覆寫驗證方法以添加特定驗證
    public override IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        // 特定驗證邏輯
        foreach (var result in base.Validate(context))
            yield return result;
    }
}
```

#### 3. **型別安全的轉換**

```csharp
// 隱含轉換
public static implicit operator FhirString?(string? value) => 
    value == null ? null : new FhirString(value);

public static implicit operator string?(FhirString? fhirString) => 
    fhirString?.Value;
```

## 優點

### 1. **型別安全**
- 強型別設計，編譯時期檢查
- 避免執行時期錯誤

### 2. **程式碼重用**
- 通用邏輯在基礎類別中實作
- 子類別只需實作特定邏輯

### 3. **可維護性**
- 清晰的架構層次
- 統一的實作模式

### 4. **可擴展性**
- 新增基本型別只需繼承 `PrimitiveTypeBase<T>`
- 實作三個抽象方法即可

### 5. **開發者體驗**
- 簡化的 API
- 豐富的 IntelliSense 支援
- 完整的文件註解

## 使用範例

### 1. **基本使用**

```csharp
// 建立實例
var fhirString = new FhirString("Hello World");
var fhirInteger = new FhirInteger(42);

// 隱含轉換
FhirString str = "Hello";  // 自動轉換
string value = str;         // 自動轉換
```

### 2. **驗證使用**

```csharp
// 驗證物件
var validationContext = new ValidationContext(fhirString);
var results = fhirString.Validate(validationContext);

foreach (var result in results)
{
    Console.WriteLine($"Validation Error: {result.ErrorMessage}");
}
```

### 3. **序列化使用**

```csharp
// JSON 序列化
var jsonValue = fhirString.ToJsonValue();
var jsonString = JsonSerializer.Serialize(fhirString);

// JSON 反序列化
fhirString.FromJsonValue(jsonValue);
```

## 遷移指南

### 1. **移除舊檔案**
```bash
# 移除有問題的檔案
rm Fhir.TypeFramework/Base/PrimitiveTypeImp.cs
```

### 2. **更新現有基本型別**
- 將所有基本型別改為繼承 `PrimitiveTypeBase<T>`
- 實作三個抽象方法
- 移除重複的程式碼

### 3. **更新測試**
- 更新單元測試以使用新的架構
- 確保所有驗證邏輯正確

## 未來擴展

### 1. **更多基本型別**
```csharp
public class FhirBoolean : PrimitiveTypeBase<bool> { }
public class FhirDecimal : PrimitiveTypeBase<decimal> { }
public class FhirDateTime : PrimitiveTypeBase<DateTime> { }
public class FhirUri : PrimitiveTypeBase<string> { }
```

### 2. **自訂驗證規則**
```csharp
public class FhirEmail : PrimitiveTypeBase<string>
{
    protected override bool IsValidValue(string? value)
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

這個重構解決了原始架構的所有問題：
- ✅ 消除了編譯錯誤
- ✅ 提供了清晰的架構層次
- ✅ 改善了型別安全
- ✅ 簡化了開發流程
- ✅ 提高了可維護性

新的架構為 FHIR SDK 提供了堅實的基礎，讓開發者能夠輕鬆建立和使用 FHIR 基本型別。 