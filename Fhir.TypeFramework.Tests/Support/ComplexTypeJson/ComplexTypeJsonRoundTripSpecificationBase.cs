using System.Text.Json;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Serialization;

namespace Fhir.TypeFramework.Tests.Support.ComplexTypeJson;

/// <summary>
/// Complex Type JSON 測試的共用骨架：各資料型別僅需提供範例 JSON 與欄位斷言。
/// </summary>
public abstract class ComplexTypeJsonRoundTripSpecificationBase<T> : IComplexTypeJsonRoundTripSpecification
    where T : DataType, new()
{
    public abstract string Name { get; }

    /// <summary>FHIR 慣例 JSON（UTF-8 字串）。</summary>
    protected abstract string FhirJsonSample { get; }

    protected abstract void AssertDecodedInstance(T instance);

    /// <summary>可選：檢查序列化後 JSON 是否含預期片段（預設驗證仍可 parse 為 JsonDocument）。</summary>
    protected virtual void AssertSerializedJsonShape(string json)
    {
        using var doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.ValueKind == JsonValueKind.Object);
    }

    public void VerifyDeserializeFromFhirJson()
    {
        var instance = FhirJsonSerializer.Deserialize<T>(FhirJsonSample);
        Assert.NotNull(instance);
        AssertDecodedInstance(instance!);
    }

    public void VerifySerializeProducesFhirShapedJson()
    {
        var instance = FhirJsonSerializer.Deserialize<T>(FhirJsonSample);
        Assert.NotNull(instance);
        var json = FhirJsonSerializer.Serialize(instance!);
        Assert.False(string.IsNullOrWhiteSpace(json));
        AssertSerializedJsonShape(json);
    }

    public void VerifyRoundTrip()
    {
        var first = FhirJsonSerializer.Deserialize<T>(FhirJsonSample);
        Assert.NotNull(first);
        AssertDecodedInstance(first!);

        var json = FhirJsonSerializer.Serialize(first!);
        var second = FhirJsonSerializer.Deserialize<T>(json);
        Assert.NotNull(second);
        AssertDecodedInstance(second!);
    }
}
