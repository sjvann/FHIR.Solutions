# Fhir.TypeFramework

`Fhir.TypeFramework` 是用於 FHIR 型別系統的 .NET 基礎類別與工具集合，提供：

- `Bases/`：`Base`、`Element`、`DataType`、`PrimitiveType<T>`、`ComplexTypeBase` 等核心繼承層級
- `DataTypes/`：FHIR Primitive/Complex 資料型別（逐步擴充中）
- `Validation/` + `Extensions/`：可重用的驗證工具與便利擴展方法（`IsValid()`、`GetValidationErrors()`、`ValidateAndThrow()`、Extension 快速操作）
- `Performance/`：可選的效能優化（快取、批次驗證、深拷貝優化、監控）
- `Serialization/`：`FhirJsonSerializer`（System.Text.Json）
- `Development/`：開發輔助工具與使用範例

## 目標框架

- `net10.0`

## 快速開始

```csharp
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.Extensions;

var id = new FhirId("patient-123");
var ok = id.IsValid();

id.ValidateAndThrow();

id.CreateExtension("http://example.com/custom", "customValue");
```

## 進一步文件

專案內的設計/實作規劃文件在：

- `README_FHIR_R5_TypeFramework_Implementation.md`
- `README_Architecture_Improvement_Plan.md`
- `README_Hybrid_Validation_Implementation.md`
- `README_Performance_Optimization_Implementation.md`
- `README_Development_Experience_Improvement.md`

