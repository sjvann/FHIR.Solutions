using Fhir.TypeFramework.Tests.Support.ComplexTypeJson.Specifications;

namespace Fhir.TypeFramework.Tests.Support.ComplexTypeJson;

/// <summary>
/// 集中註冊要跑 JSON 往返的 Complex「深化」規格（欄位級斷言）。
/// 組件內<strong>所有</strong>具體 DataTypes 的 JSON 邊界至少會由 <c>Inventory/ModelTypeJsonRoundTripTests</c> 逐型別 Theory 涵蓋。
/// </summary>
public static class ComplexTypeJsonSpecificationRegistry
{
    public static IReadOnlyList<IComplexTypeJsonRoundTripSpecification> All { get; } =
    [
        new CodingJsonSpecification(),
        new PeriodJsonSpecification(),
        new CodeableConceptJsonSpecification(),
        new MetaJsonSpecification()
    ];
}
