# FHIR Type Framework 架構改善建議方案

## 📋 **專案現況分析**

### **🏗️ 當前架構優點**
1. **清晰的層次結構**：`Base` → `Element` → `DataType` → `PrimitiveType` 的繼承關係清晰
2. **介面導向設計**：使用 `ITypeFramework` 等介面提供良好抽象
3. **解決循環依賴**：透過介面分離解決了 Extension 的循環依賴問題
4. **型別安全**：使用泛型和約束確保型別安全

### **🔍 識別的問題**
1. **高度重複的程式碼模式**
   - 每個 Primitive Type 都有相似的建構函式、隱式轉換、驗證邏輯
   - Complex Types 也有重複的 `DeepCopy()`、`IsExactly()`、`Validate()` 實作

2. **缺乏統一的基礎類別**
   - 雖然有 `PrimitiveType` 基礎類別，但具體實作仍有很多重複
   - Complex Types 沒有統一的基礎類別

3. **驗證邏輯分散**
   - 每個類別都有自己的驗證邏輯
   - 缺乏統一的驗證框架

## 🎯 **改善目標**

### **主要目標**
1. **減少重複程式碼**：目標減少 70% 的重複程式碼
2. **提升維護性**：統一的基礎類別和驗證框架
3. **改善開發體驗**：簡化新增型別的流程
4. **保持型別安全**：維持現有的型別安全設計

### **核心原則**
1. **FHIR 命名規範**：所有 FHIR Primitive Type 必須加上 `Fhir` 前綴
2. **型別安全**：除了 Primitive Type 外，所有 FHIR 相關類別都使用加前綴的資料型態
3. **DocFX 規範**：所有類別函數都要有符合 DocFX 規範的註解
4. **命名空間檢查**：確保所有 using 和 namespace 都正確
5. **目錄命名**：所有目錄都使用複數型態
6. **Complex Type 定義**：Complex Type 是 Reference Type，包含多個欄位
7. **Choice Type 支援**：使用 OneOf 套件處理 [x] 欄位定義

## 🏗️ **改善方案架構**

### **方案一：建立統一的基礎類別層次**

#### **1.1 泛型基礎類別**
```csharp
// 已建立：PrimitiveTypeBase<TValue>
// 已建立：ComplexTypeBase
// 已建立：StringPrimitiveTypeBase
// 已建立：NumericPrimitiveTypeBase<TNumeric>
// 已建立：BooleanPrimitiveTypeBase
// 已建立：DateTimePrimitiveTypeBase<TDateTime>
```

#### **1.2 新的繼承層次**
```
Base (所有型別的根)
├── Element (資源內元素的基礎)
│   └── ComplexTypeBase (複雜型別基礎)
│       ├── CodeableConcept
│       ├── Reference
│       └── ...
├── DataType (資料型別基礎)
│   └── PrimitiveType (基本型別基礎)
│       ├── PrimitiveTypeBase<TValue> (泛型基礎)
│       │   ├── StringPrimitiveTypeBase (字串型別基礎)
│       │   ├── NumericPrimitiveTypeBase<TNumeric> (數值型別基礎)
│       │   ├── BooleanPrimitiveTypeBase (布林型別基礎)
│       │   └── DateTimePrimitiveTypeBase<TDateTime> (日期時間型別基礎)
│       └── 具體實作類別
└── Resource (資源基礎)
```

### **方案二：建立專用的基礎類別**

#### **2.1 字串型別基礎類別**
- **適用範圍**：FhirString, FhirUri, FhirUrl, FhirCode, FhirId, FhirCanonical, FhirOid, FhirUuid, FhirMarkdown
- **提供功能**：統一的字串驗證、隱式轉換、工廠方法

#### **2.2 數值型別基礎類別**
- **適用範圍**：FhirInteger, FhirPositiveInt, FhirUnsignedInt, FhirInteger64, FhirDecimal
- **提供功能**：統一的數值解析、範圍驗證、型別安全轉換

#### **2.3 布林型別基礎類別**
- **適用範圍**：FhirBoolean
- **提供功能**：統一的布林值處理邏輯、多種表示法支援

#### **2.4 日期時間型別基礎類別**
- **適用範圍**：FhirDateTime, FhirDate, FhirTime, FhirInstant
- **提供功能**：統一的日期時間解析、格式驗證、時區處理

### **方案三：建立程式碼產生器模板**

#### **3.1 Primitive Type 模板**
```csharp
// 已建立：PrimitiveTypeTemplate.cs
// 提供變數替換功能：
// - {ClassName}: 類別名稱
// - {BaseClass}: 基礎類別名稱
// - {ValueType}: 值型別
// - {ParseValueMethod}: 解析方法名稱
// - {ValidateValueMethod}: 驗證方法名稱
```

#### **3.2 Complex Type 模板**
```csharp
// 已建立：ComplexTypeTemplate.cs
// 提供變數替換功能：
// - {ClassName}: 類別名稱
// - {Properties}: 屬性定義
// - {DeepCopyLogic}: 深層複製邏輯
// - {ValidationLogic}: 驗證邏輯
```

### **方案四：建立驗證框架**

#### **4.1 統一驗證規則**
```csharp
// 已建立：ValidationFramework.cs
// 提供：
// - 字串驗證規則（長度、格式、URI等）
// - 數值驗證規則（範圍、精度等）
// - 日期時間驗證規則
// - 複雜型別驗證規則
```

#### **4.2 驗證屬性**
```csharp
// 已建立：ValidationAttributes.cs
// 提供：
// - [FhirString] 屬性
// - [FhirInteger] 屬性
// - [FhirUri] 屬性
// - [FhirDate] 屬性
// - [FhirRequired] 屬性
```

## 📊 **預期效益分析**

### **程式碼減少統計**
| 類別類型 | 重構前平均行數 | 重構後平均行數 | 減少比例 |
|----------|----------------|----------------|----------|
| Primitive Types | 80-100 行 | 20-30 行 | 70-75% |
| Complex Types | 150-200 行 | 50-80 行 | 60-65% |
| 總體平均 | 120-150 行 | 35-55 行 | 65-70% |

### **維護成本降低**
1. **新增型別時間**：從 2-3 小時減少到 30 分鐘
2. **修改基礎邏輯**：從需要修改 20+ 檔案減少到 1-2 檔案
3. **測試覆蓋率**：從 60% 提升到 90%+

### **開發體驗改善**
1. **IntelliSense 支援**：更完整的 API 提示
2. **錯誤訊息**：更清晰的驗證錯誤訊息
3. **文件完整性**：100% 的 API 文件覆蓋率

## 🚀 **實作步驟**

### **階段一：基礎架構建立（已完成）**
1. ✅ 建立 `PrimitiveTypeBase<TValue>`
2. ✅ 建立 `ComplexTypeBase`
3. ✅ 建立 `StringPrimitiveTypeBase`
4. ✅ 建立 `NumericPrimitiveTypeBase<TNumeric>`
5. ✅ 建立 `BooleanPrimitiveTypeBase`
6. ✅ 建立 `DateTimePrimitiveTypeBase<TDateTime>`
7. ✅ 建立 `ValidationFramework`
8. ✅ 建立 `ValidationAttributes`

### **階段二：重構現有型別**
1. **重構 Primitive Types**
   ```csharp
   // 範例：重構 FhirString
   public class FhirString : StringPrimitiveTypeBase
   {
       public FhirString() { }
       public FhirString(string? value) : base(value) { }
       
       public static implicit operator FhirString?(string? value) => CreateFromString<FhirString>(value);
       public static implicit operator string?(FhirString? instance) => GetStringValue(instance);
       
       protected override bool ValidateStringValue(string? value) => ValidationFramework.ValidateStringByteSize(value, 1024 * 1024);
   }
   ```

2. **重構 Complex Types**
   ```csharp
   // 範例：重構 CodeableConcept
   public class CodeableConcept : ComplexTypeBase
   {
       public IList<Coding>? Coding { get; set; }
       public FhirString? Text { get; set; }
       
       protected override void DeepCopyInternal(ComplexTypeBase copy)
       {
           var typedCopy = (CodeableConcept)copy;
           typedCopy.Coding = DeepCopyList(Coding);
           typedCopy.Text = Text?.DeepCopy() as FhirString;
       }
       
       protected override bool IsExactlyInternal(ComplexTypeBase other)
       {
           var typedOther = (CodeableConcept)other;
           return Equals(Text, typedOther.Text) && AreListsEqual(Coding, typedOther.Coding);
       }
       
       protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
       {
           return ValidateList(Coding, validationContext);
       }
   }
   ```

### **階段三：建立程式碼產生器**
1. **建立模板引擎**
2. **建立 CLI 工具**
3. **整合到 Visual Studio 擴充功能**

### **階段四：測試和文件**
1. **單元測試覆蓋率達到 90%+**
2. **整合測試確保向後相容性**
3. **完整的 API 文件**
4. **使用範例和最佳實踐指南**

## 🔧 **技術實作細節**

### **泛型約束設計**
```csharp
// 數值型別約束
public abstract class NumericPrimitiveTypeBase<TNumeric> : PrimitiveTypeBase<TNumeric>
    where TNumeric : struct, IComparable<TNumeric>, IEquatable<TNumeric>

// 字串型別約束
public abstract class StringPrimitiveTypeBase : PrimitiveTypeBase<string>

// 日期時間型別約束
public abstract class DateTimePrimitiveTypeBase<TDateTime> : PrimitiveTypeBase<TDateTime>
    where TDateTime : struct, IComparable<TDateTime>, IEquatable<TDateTime>
```

### **工廠方法模式**
```csharp
// 統一的建立方法
protected static T? CreateFromValue<T>(TNumeric? value) where T : NumericPrimitiveTypeBase<TNumeric>, new()
protected static T? CreateFromString<T>(string? value) where T : StringPrimitiveTypeBase, new()
protected static T? CreateFromBoolean<T>(bool? value) where T : BooleanPrimitiveTypeBase, new()
```

### **驗證框架整合**
```csharp
// 使用統一的驗證規則
protected override bool ValidateStringValue(string? value)
{
    return ValidationFramework.ValidateStringByteSize(value, 1024 * 1024) &&
           ValidationFramework.ValidateStringLength(value, 1000);
}
```

### **Choice Type 支援**
```csharp
// 使用 OneOf 套件處理 Choice Type
public class ExtensionValueChoice : ChoiceType<
    FhirString, FhirInteger, FhirBoolean, FhirDecimal, FhirDateTime
>
{
    // 隱含轉換運算子
    public static implicit operator ExtensionValueChoice(FhirString value) => new(value);
    public static implicit operator ExtensionValueChoice(FhirInteger value) => new(value);
    // ... 其他轉換運算子
}
```

## 📈 **風險評估和緩解策略**

### **風險 1：向後相容性**
- **風險等級**：中
- **緩解策略**：保持現有 API 不變，新功能作為擴展

### **風險 2：效能影響**
- **風險等級**：低
- **緩解策略**：使用泛型避免裝箱拆箱，優化深層複製邏輯

### **風險 3：學習曲線**
- **風險等級**：低
- **緩解策略**：提供完整文件和使用範例

## ✅ **成功指標**

### **量化指標**
1. **程式碼減少**：重複程式碼減少 70%+
2. **維護成本**：新增型別時間減少 80%
3. **測試覆蓋率**：達到 90%+
4. **文件完整性**：100% API 文件覆蓋率

### **質化指標**
1. **開發者滿意度**：提升開發效率
2. **錯誤率降低**：減少執行時期錯誤
3. **程式碼品質**：提升可讀性和可維護性

## 🎯 **結論**

這個改善方案將大幅提升 FHIR Type Framework 的：
- **程式碼重用性**：減少 70% 的重複程式碼
- **維護性**：統一的基礎類別和驗證框架
- **開發體驗**：簡化的 API 和完整的工具支援
- **型別安全**：保持現有的型別安全設計
- **FHIR 規範符合性**：確保所有命名和結構符合 FHIR R5 規範

透過分階段實作，可以確保向後相容性並逐步改善整個架構，為建立一個完整的 FHIR SDK 奠定堅實的基礎。 