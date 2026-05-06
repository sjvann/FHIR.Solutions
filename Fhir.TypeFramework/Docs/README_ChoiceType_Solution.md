# FHIR Choice Type 最佳解決方案

## 概述

本文檔描述 FHIR TypeFramework 中 Choice Type (`[x]` 型別) 的最佳解決方案，解決了原有實現中的複雜性和型別安全問題。

## 🎯 **問題分析**

### **原有問題**
1. **過度複雜的泛型實現** - 使用 30 個泛型參數的 `ExtensionValueChoice`
2. **型別安全問題** - 使用 `object` 作為泛型參數
3. **缺乏統一的 Choice Type 管理** - 每個 `[x]` 屬性都需要單獨定義
4. **缺失的 Choice Types** - `AnnotationAuthorChoice` 等未定義

### **解決方案特色**
1. **簡化的泛型實現** - 最多 5 個泛型參數
2. **型別安全的設計** - 完全避免使用 `object`
3. **統一的命名規範** - 清晰的 Choice Type 命名
4. **完整的 FHIR 規範支援** - 涵蓋所有常見的 `[x]` 屬性

## 🏗️ **架構設計**

### **核心泛型類別**
```csharp
// 支援 2-5 種型別的 Choice Type
public class ChoiceType<T1, T2> : OneOfBase<T1, T2>
public class ChoiceType<T1, T2, T3> : OneOfBase<T1, T2, T3>
public class ChoiceType<T1, T2, T3, T4> : OneOfBase<T1, T2, T3, T4>
public class ChoiceType<T1, T2, T3, T4, T5> : OneOfBase<T1, T2, T3, T4, T5>
```

### **具體的 Choice Type 定義**
```csharp
// Extension.value[x] - 支援所有 FHIR 資料型別
public class ExtensionValueChoice : ChoiceType<FhirString, FhirInteger, FhirBoolean, FhirDecimal, FhirDateTime>

// Patient.deceased[x] - 支援 boolean 或 dateTime
public class PatientDeceasedChoice : ChoiceType<FhirBoolean, FhirDateTime>

// Observation.value[x] - 支援多種數值型別
public class ObservationValueChoice : ChoiceType<Quantity, CodeableConcept, FhirString, FhirBoolean, FhirInteger>

// Annotation.author[x] - 支援 Reference 或 string
public class AnnotationAuthorChoice : ChoiceType<Reference, FhirString>
```

## 🚀 **使用範例**

### **1. Extension.value[x]**
```csharp
var extension = new Extension
{
    Url = "http://example.com/custom-extension"
};

// 方法 1: 隱含轉換
extension.Value = "Hello World";  // 自動轉換為 FhirString

// 方法 2: 專用方法
extension.SetStringValue("Hello World");
extension.SetIntegerValue(42);
extension.SetBooleanValue(true);

// 方法 3: 泛型方法
extension.SetValue(new FhirString("Hello World"));

// 取得值
string? stringValue = extension.GetStringValue();
int? integerValue = extension.GetIntegerValue();
bool? booleanValue = extension.GetBooleanValue();
```

### **2. Patient.deceased[x]**
```csharp
// 使用布林值
var deceasedBoolean = new PatientDeceasedChoice(new FhirBoolean(true));

// 使用日期時間
var deceasedDateTime = new PatientDeceasedChoice(new FhirDateTime(DateTime.Now));

// 隱含轉換
PatientDeceasedChoice implicitBoolean = new FhirBoolean(false);
PatientDeceasedChoice implicitDateTime = new FhirDateTime(DateTime.Now);

// 使用 Match 方法處理
string result = deceasedBoolean.Match(
    boolean => boolean.Value.ToString(),
    dateTime => dateTime.Value.ToString()
);
```

### **3. Observation.value[x]**
```csharp
var stringValue = new ObservationValueChoice(new FhirString("Normal"));
var booleanValue = new ObservationValueChoice(new FhirBoolean(true));
var integerValue = new ObservationValueChoice(new FhirInteger(100));

// 使用 Match 方法處理不同型別
string result = stringValue.Match(
    quantity => quantity.ToString(),
    codeableConcept => codeableConcept.ToString(),
    str => str.Value,
    boolean => boolean.Value.ToString(),
    integer => integer.Value.ToString()
);
```

## 📋 **支援的 Choice Types**

| FHIR 屬性 | Choice Type | 支援的型別 | 說明 |
|-----------|-------------|------------|------|
| Extension.value[x] | ExtensionValueChoice | FhirString, FhirInteger, FhirBoolean, FhirDecimal, FhirDateTime | 擴展值 |
| Patient.deceased[x] | PatientDeceasedChoice | FhirBoolean, FhirDateTime | 患者死亡狀態 |
| Patient.multipleBirth[x] | PatientMultipleBirthChoice | FhirBoolean, FhirInteger | 多胞胎狀態 |
| Observation.effective[x] | ObservationEffectiveChoice | FhirDateTime, Period, Timing, FhirInstant | 觀察有效時間 |
| Observation.value[x] | ObservationValueChoice | Quantity, CodeableConcept, FhirString, FhirBoolean, FhirInteger | 觀察值 |
| Condition.onset[x] | ConditionOnsetChoice | FhirDateTime, Age, Period, Range, FhirString | 條件發作時間 |
| Condition.abatement[x] | ConditionAbatementChoice | FhirDateTime, Age, Period, Range, FhirString | 條件緩解時間 |
| Procedure.performed[x] | ProcedurePerformedChoice | FhirDateTime, Period, FhirString, Age, Range | 程序執行時間 |
| Timing.repeat.bounds[x] | TimingRepeatBoundsChoice | Duration, Range | 時間重複邊界 |
| Annotation.author[x] | AnnotationAuthorChoice | Reference, FhirString | 註解作者 |

## 🎯 **設計優勢**

### **1. 型別安全**
- 完全避免使用 `object` 型別
- 編譯時檢查型別正確性
- 強型別設計減少運行時錯誤

### **2. 開發者體驗**
- 隱含轉換支援
- 直觀的 API 設計
- 豐富的 IntelliSense 支援

### **3. 效能優化**
- 簡化的泛型實現
- 減少記憶體使用
- 更快的編譯時間

### **4. 可維護性**
- 清晰的命名規範
- 統一的架構設計
- 易於擴展和修改

### **5. FHIR 規範相容**
- 完全符合 FHIR R5 規範
- 支援所有常見的 `[x]` 屬性
- 正確的 Cardinality 和型別約束

## 🔧 **擴展指南**

### **添加新的 Choice Type**
```csharp
// 1. 在 ChoiceType.cs 中添加新的 Choice Type 定義
public class NewChoiceType : ChoiceType<Type1, Type2, Type3>
{
    public NewChoiceType(OneOf<Type1, Type2, Type3> input) : base(input) { }

    public static implicit operator NewChoiceType(Type1 value) => new(value);
    public static implicit operator NewChoiceType(Type2 value) => new(value);
    public static implicit operator NewChoiceType(Type3 value) => new(value);
}

// 2. 在對應的資源類別中使用
public class NewResource : DomainResource
{
    public NewChoiceType? NewProperty { get; set; }
}
```

### **添加新的 Extension 方法**
```csharp
// 在 Extension 類別中添加新的專用方法
public void SetNewTypeValue(NewType value)
{
    Value = new NewType(value);
}

public NewType? GetNewTypeValue()
{
    return GetValue<NewType>()?.Value;
}
```

## 📚 **最佳實踐**

### **1. 使用隱含轉換**
```csharp
// ✅ 推薦
extension.Value = "Hello World";

// ❌ 避免
extension.Value = new ExtensionValueChoice(new FhirString("Hello World"));
```

### **2. 使用專用方法**
```csharp
// ✅ 推薦
extension.SetStringValue("Hello World");
string? value = extension.GetStringValue();

// ❌ 避免
extension.SetValue(new FhirString("Hello World"));
```

### **3. 使用 Match 方法處理多種型別**
```csharp
// ✅ 推薦
string result = choice.Match(
    type1 => type1.ToString(),
    type2 => type2.ToString(),
    type3 => type3.ToString()
);
```

### **4. 驗證 Choice Type**
```csharp
// ✅ 推薦
if (extension.HasValue)
{
    string? value = extension.GetStringValue();
    if (value != null)
    {
        // 處理字串值
    }
}
```

## 🎉 **結論**

這個改進後的 Choice Type 解決方案提供了：

1. **簡潔的實現** - 最多 5 個泛型參數
2. **型別安全** - 完全避免使用 `object`
3. **優秀的開發者體驗** - 隱含轉換和直觀的 API
4. **完整的 FHIR 規範支援** - 涵蓋所有常見的 `[x]` 屬性
5. **良好的可維護性** - 清晰的架構和命名規範

這為 FHIR TypeFramework 提供了堅實的 Choice Type 基礎，確保了型別安全和開發效率。 