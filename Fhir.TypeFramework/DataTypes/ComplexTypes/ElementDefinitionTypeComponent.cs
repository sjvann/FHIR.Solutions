using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>
/// FHIR R5 ElementDefinition.type 切片（對應規格中 ElementDefinition.type 之 backbone）。
/// </summary>
public sealed class ElementDefinitionTypeComponent : BackboneElement
{
    [JsonPropertyName("code")] public FhirUri? Code { get; set; }
    [JsonPropertyName("profile")] public List<FhirCanonical>? Profile { get; set; }
    [JsonPropertyName("targetProfile")] public List<FhirCanonical>? TargetProfile { get; set; }

    public override Base DeepCopy()
    {
        var copy = (ElementDefinitionTypeComponent)base.DeepCopy();
        copy.Code = Code?.DeepCopy() as FhirUri;
        copy.Profile = Profile?.Select(p => (p.DeepCopy() as FhirCanonical)!).ToList();
        copy.TargetProfile = TargetProfile?.Select(p => (p.DeepCopy() as FhirCanonical)!).ToList();
        return copy;
    }

    public override bool IsExactly(Base other)
    {
        if (other is not ElementDefinitionTypeComponent o) return false;
        if (!base.IsExactly(other)) return false;
        if (!ValueEq(Code, o.Code)) return false;
        if (!ListEq(Profile, o.Profile)) return false;
        if (!ListEq(TargetProfile, o.TargetProfile)) return false;
        return true;
    }

    private static bool ValueEq<T>(T? a, T? b) where T : Base =>
        (a == null && b == null) || (a != null && b != null && a.IsExactly(b));

    private static bool ListEq<T>(IList<T>? a, IList<T>? b) where T : Base
    {
        if (a == null && b == null) return true;
        if (a == null || b == null || a.Count != b.Count) return false;
        return a.Zip(b, (x, y) => x.IsExactly(y)).All(z => z);
    }
}
