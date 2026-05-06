using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.Bases;

/// <summary>
/// This is the base resource type for everything.
/// 所有 FHIR Resources 的基礎類別（對應規格 <c>Resource</c>）。
/// </summary>
/// <remarks>
/// FHIR R5 Resource (Abstract)
/// Structure:
/// - id: id (0..1)
/// - meta: Meta (0..1)
/// - implicitRules: uri (0..1)
/// - language: code (0..1)
/// </remarks>
public abstract class Resource : Base, IIdentifiableTypeFramework
{
    /// <summary>
    /// Logical id of this artifact（對應規格之 id primitive）。
    /// </summary>
    [JsonPropertyName("id")]
    public FhirId? Id { get; set; }

    /// <summary>
    /// Metadata about the resource.
    /// </summary>
    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }

    /// <summary>
    /// A set of rules under which this content was created.
    /// </summary>
    [JsonPropertyName("implicitRules")]
    public FhirUri? ImplicitRules { get; set; }

    /// <summary>
    /// Language of the resource content.
    /// </summary>
    [JsonPropertyName("language")]
    public FhirCode? Language { get; set; }

    /// <inheritdoc />
    public override Base DeepCopy()
    {
        var copy = (Resource)MemberwiseClone();
        copy.Id = Id?.DeepCopy() as FhirId;
        copy.Meta = Meta?.DeepCopy() as Meta;
        copy.ImplicitRules = ImplicitRules?.DeepCopy() as FhirUri;
        copy.Language = Language?.DeepCopy() as FhirCode;

        return copy;
    }

    /// <inheritdoc />
    public override bool IsExactly(Base other)
    {
        if (other is not Resource otherResource)
            return false;

        return ValueEq(Id, otherResource.Id)
               && ValueEq(Meta, otherResource.Meta)
               && ValueEq(ImplicitRules, otherResource.ImplicitRules)
               && ValueEq(Language, otherResource.Language);
    }

    private static bool ValueEq<T>(T? a, T? b) where T : Base =>
        (a == null && b == null) || (a != null && b != null && a.IsExactly(b));

    /// <inheritdoc />
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Id != null)
        {
            foreach (var result in Id.Validate(validationContext))
                yield return result;
        }

        if (Meta != null)
        {
            foreach (var result in Meta.Validate(new ValidationContext(Meta)))
                yield return result;
        }

        if (ImplicitRules != null)
        {
            foreach (var result in ImplicitRules.Validate(validationContext))
                yield return result;
        }

        if (Language != null)
        {
            foreach (var result in Language.Validate(validationContext))
                yield return result;
        }
    }
}
