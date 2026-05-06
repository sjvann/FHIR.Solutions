using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Fhir.TypeFramework.Serialization;
using Fhir.TypeFramework.Tests.Application.Contracts;
using Fhir.TypeFramework.Tests.Support.ComplexTypeJson;

namespace Fhir.TypeFramework.Tests.Application.BehaviorSuites;

public sealed class ComplexTypeJsonBehaviorSuite : IComplexTypeJsonBehaviorSuite
{
    public void VerifyAllComplexTypesDeserializeFromFhirJson()
    {
        foreach (var spec in ComplexTypeJsonSpecificationRegistry.All)
            spec.VerifyDeserializeFromFhirJson();
    }

    public void VerifyAllComplexTypesSerializeToFhirShapedJson()
    {
        foreach (var spec in ComplexTypeJsonSpecificationRegistry.All)
            spec.VerifySerializeProducesFhirShapedJson();
    }

    public void VerifyAllComplexTypesRoundTrip()
    {
        foreach (var spec in ComplexTypeJsonSpecificationRegistry.All)
            spec.VerifyRoundTrip();
    }

    public void VerifyPrimitiveJsonConverterLegacyObjectForm()
    {
        var legacy = """{"value":"legacy-string"}""";
        var s = FhirJsonSerializer.Deserialize<FhirString>(legacy);
        Assert.NotNull(s);
        Assert.Equal("legacy-string", s!.StringValue);
    }

    public void VerifyPrimitiveJsonConverterReadsBooleanAndNumber()
    {
        var b = FhirJsonSerializer.Deserialize<FhirBoolean>("true");
        Assert.NotNull(b);
        Assert.True(b!.Value);

        var i = FhirJsonSerializer.Deserialize<FhirInteger>("42");
        Assert.NotNull(i);
        Assert.Equal(42, i!.Value);
    }

    public void VerifyPrimitiveJsonConverterReadsNullLongAndScientific()
    {
        Assert.Null(FhirJsonSerializer.Deserialize<FhirString>("null"));

        var l = FhirJsonSerializer.Deserialize<FhirInteger64>("3000000000");
        Assert.NotNull(l);
        Assert.Equal(3_000_000_000L, l!.Value);

        var d = FhirJsonSerializer.Deserialize<FhirDecimal>("1e3");
        Assert.NotNull(d);
        Assert.Equal(1000m, d!.Value);
    }

    public void VerifyPrimitiveJsonConverterNumericShapes()
    {
        var fromObjectNumber = FhirJsonSerializer.Deserialize<FhirInteger>("""{"value": 99}""");
        Assert.NotNull(fromObjectNumber);
        Assert.Equal(99, fromObjectNumber!.Value);

        var highPrecision = FhirJsonSerializer.Deserialize<FhirDecimal>("1.23456789012345");
        Assert.NotNull(highPrecision);

        var decJson = FhirJsonSerializer.Serialize(new FhirDecimal(200.5m));
        Assert.Contains("200.5", decJson, StringComparison.Ordinal);

        var longJson = FhirJsonSerializer.Serialize(new FhirInteger64(3_000_000_000L));
        Assert.Contains("3000000000", longJson, StringComparison.Ordinal);

        var emptyStringJson = FhirJsonSerializer.Serialize(new FhirString());
        Assert.Equal("null", emptyStringJson.Trim());

        Assert.Throws<System.Text.Json.JsonException>(() => FhirJsonSerializer.Deserialize<FhirString>("["));

        var legacyValueNull = FhirJsonSerializer.Deserialize<FhirString>("""{"value": null}""");
        Assert.NotNull(legacyValueNull);

        var extreme = FhirJsonSerializer.Deserialize<FhirDecimal>("1e308");
        Assert.NotNull(extreme);
    }
}
