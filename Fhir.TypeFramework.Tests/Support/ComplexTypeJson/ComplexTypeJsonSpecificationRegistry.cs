using Fhir.TypeFramework.Tests.Support.ComplexTypeJson.Specifications;

namespace Fhir.TypeFramework.Tests.Support.ComplexTypeJson;

/// <summary>
/// 集中註冊要跑 JSON 往返的 Complex 規格（新增資料型別時只須加一筆）。
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
