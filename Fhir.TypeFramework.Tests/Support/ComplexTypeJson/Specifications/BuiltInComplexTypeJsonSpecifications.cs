using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.Tests.Support.ComplexTypeJson.Specifications;

public sealed class CodingJsonSpecification : ComplexTypeJsonRoundTripSpecificationBase<Coding>
{
    public override string Name => "Coding";

    protected override string FhirJsonSample => """
        {
          "system": "http://loinc.org",
          "version": "2.73",
          "code": "8867-4",
          "display": null,
          "userSelected": false
        }
        """;

    protected override void AssertDecodedInstance(Coding instance)
    {
        Assert.Equal("http://loinc.org", instance.System?.StringValue);
        Assert.Equal("2.73", instance.Version?.StringValue);
        Assert.Equal("8867-4", instance.Code?.StringValue);
        Assert.Null(instance.Display);
        Assert.False(instance.UserSelected?.Value ?? true);
    }

    protected override void AssertSerializedJsonShape(string json)
    {
        base.AssertSerializedJsonShape(json);
        Assert.Contains("http://loinc.org", json, StringComparison.Ordinal);
        Assert.Contains("8867-4", json, StringComparison.Ordinal);
        Assert.Contains("\"userSelected\": false", json, StringComparison.Ordinal);
        Assert.DoesNotContain("Heart rate", json, StringComparison.Ordinal);
    }
}

public sealed class PeriodJsonSpecification : ComplexTypeJsonRoundTripSpecificationBase<Period>
{
    public override string Name => "Period";

    protected override string FhirJsonSample => """
        {
          "start": "2020-01-01T00:00:00Z",
          "end": "2020-12-31T23:59:59Z"
        }
        """;

    protected override void AssertDecodedInstance(Period instance)
    {
        Assert.Equal("2020-01-01T00:00:00Z", instance.Start?.StringValue);
        Assert.Equal("2020-12-31T23:59:59Z", instance.End?.StringValue);
    }
}

public sealed class CodeableConceptJsonSpecification : ComplexTypeJsonRoundTripSpecificationBase<CodeableConcept>
{
    public override string Name => "CodeableConcept";

    protected override string FhirJsonSample => """
        {
          "coding": [
            {
              "system": "http://snomed.info/sct",
              "code": "386661006",
              "display": "Fever"
            }
          ],
          "text": "發燒"
        }
        """;

    protected override void AssertDecodedInstance(CodeableConcept instance)
    {
        Assert.NotNull(instance.Coding);
        Assert.Single(instance.Coding!);
        Assert.Equal("http://snomed.info/sct", instance.Coding![0].System?.StringValue);
        Assert.Equal("386661006", instance.Coding[0].Code?.StringValue);
        Assert.Equal("發燒", instance.Text?.StringValue);
    }
}

/// <summary>專案內新增之 <see cref="Meta"/>，示範多欄位 Complex 與陣列欄位。</summary>
public sealed class MetaJsonSpecification : ComplexTypeJsonRoundTripSpecificationBase<Meta>
{
    public override string Name => "Meta";

    protected override string FhirJsonSample => """
        {
          "versionId": "example-version-1",
          "lastUpdated": "2020-01-01T12:00:00Z",
          "source": "https://acme.org/fhir",
          "profile": [ "http://hl7.org/fhir/StructureDefinition/Patient" ],
          "tag": [
            {
              "system": "http://terminology.hl7.org/CodeSystem/v3-ObservationValue",
              "code": "SUBSETTED",
              "display": "Resource encoded as subset"
            }
          ]
        }
        """;

    protected override void AssertDecodedInstance(Meta instance)
    {
        Assert.Equal("example-version-1", instance.VersionId?.StringValue);
        Assert.Equal("2020-01-01T12:00:00Z", instance.LastUpdated?.StringValue);
        Assert.Equal("https://acme.org/fhir", instance.Source?.StringValue);
        Assert.NotNull(instance.Profile);
        Assert.Single(instance.Profile!);
        Assert.Equal("http://hl7.org/fhir/StructureDefinition/Patient", instance.Profile![0].StringValue);
        Assert.NotNull(instance.Tag);
        Assert.Single(instance.Tag!);
        Assert.Equal("SUBSETTED", instance.Tag![0].Code?.StringValue);
    }
}
