namespace Fhir.TypeFramework.Tests.Support.ComplexTypeJson;

/// <summary>
/// 單一 Complex（FHIR DataType）之 JSON 範例與驗證契約。
/// </summary>
public interface IComplexTypeJsonRoundTripSpecification
{
    string Name { get; }

    /// <summary>由 FHIR JSON（camelCase、primitive 為純量）反序列化並驗證欄位。</summary>
    void VerifyDeserializeFromFhirJson();

    /// <summary>由記憶體模型序列化，並檢查輸出符合 FHIR JSON 慣例（關鍵片段）。</summary>
    void VerifySerializeProducesFhirShapedJson();

    /// <summary>反序列化 → 序列化 → 再反序列化，語意應一致。</summary>
    void VerifyRoundTrip();
}
