using System.Reflection;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Serialization;
using Xunit.Abstractions;

namespace Fhir.TypeFramework.Tests.Inventory;

/// <summary>
/// 逐「具體模型型別」驗證 JSON 序列化邊界：測試總管會為每個型別各顯示一筆案例。
/// </summary>
public sealed class ModelTypeJsonRoundTripTests
{
    readonly ITestOutputHelper _output;

    public ModelTypeJsonRoundTripTests(ITestOutputHelper output) => _output = output;

    public static IEnumerable<object[]> ModelTypes =>
        ModelTypeDiscovery.GetConcreteModelTypes()
            .Select(t => new object[] { t.Name, t });

    /// <summary>逐一型別案例：測試總管會展開為「JSON round-trip … (FhirString)」等形式。</summary>
    [Theory(DisplayName = "JSON round-trip")]
    [Trait("Category", "ModelInventory")]
    [MemberData(nameof(ModelTypes))]
    public void Each_concrete_model_type_round_trips_through_json_boundary(string typeShortName, Type modelType)
    {
        Assert.Equal(typeShortName, modelType.Name);

        var instance = MinimalModelFactory.CreateMinimal(modelType);
        var json = FhirJsonSerializer.Serialize(instance);
        Assert.False(string.IsNullOrWhiteSpace(json));

        var roundTrip = Deserialize(modelType, json);
        Assert.NotNull(roundTrip);

        _output.WriteLine($"{modelType.FullName}");
        _output.WriteLine(json);
    }

    [Fact]
    public void Inventory_lists_every_concrete_model_type_for_round_trip_theory()
    {
        var types = ModelTypeDiscovery.GetConcreteModelTypes();
        Assert.NotEmpty(types);

        var primitives = types.Count(t => t.FullName?.Contains(".PrimitiveTypes.", StringComparison.Ordinal) == true);
        var complex = types.Count - primitives;

        _output.WriteLine($"Concrete model types (total): {types.Count}");
        _output.WriteLine($"  PrimitiveTypes: {primitives}");
        _output.WriteLine($"  Complex / backbone / other under DataTypes: {complex}");

        foreach (var t in types)
            _output.WriteLine($"  - {t.FullName}");
    }

    static Base? Deserialize(Type type, string json)
    {
        var method = typeof(FhirJsonSerializer)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m => m.Name == nameof(FhirJsonSerializer.Deserialize) && m.IsGenericMethodDefinition);
        var gm = method.MakeGenericMethod(type);
        return (Base?)gm.Invoke(null, [json]);
    }
}
