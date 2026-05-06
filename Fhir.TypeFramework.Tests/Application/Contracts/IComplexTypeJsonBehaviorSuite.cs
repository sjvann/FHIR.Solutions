namespace Fhir.TypeFramework.Tests.Application.Contracts;

/// <summary>
/// FHIR Complex Type（<see cref="Fhir.TypeFramework.Bases.DataType"/>）之 JSON 行為。
/// </summary>
public interface IComplexTypeJsonBehaviorSuite
{
    void VerifyAllComplexTypesDeserializeFromFhirJson();

    void VerifyAllComplexTypesSerializeToFhirShapedJson();

    void VerifyAllComplexTypesRoundTrip();

    /// <summary>純量轉換器仍相容舊的 <c>{ "value": "..." }</c> 物件形式。</summary>
    void VerifyPrimitiveJsonConverterLegacyObjectForm();

    /// <summary>純量轉換器可讀取 JSON 布林與數字。</summary>
    void VerifyPrimitiveJsonConverterReadsBooleanAndNumber();

    /// <summary>純量轉換器可讀取 null、長整數與科學記號數字等分支。</summary>
    void VerifyPrimitiveJsonConverterReadsNullLongAndScientific();

    /// <summary>純量轉換器讀寫數值形態（含巢狀 value 為數字、decimal 序列化等）。</summary>
    void VerifyPrimitiveJsonConverterNumericShapes();
}
