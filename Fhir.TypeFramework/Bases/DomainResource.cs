using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.DataTypes;

namespace Fhir.TypeFramework.Bases;

/// <summary>
/// FHIR R5 DomainResource — 含文字敘述、contained 與於資源層級之 extension／modifierExtension。
/// </summary>
/// <remarks>
/// 對應規格：繼承 <see cref="Resource"/>，並新增 text、contained、extension、modifierExtension。
/// </remarks>
public abstract class DomainResource : Resource
{
    /// <summary>
    /// A human-readable summary of the resource.
    /// </summary>
    [JsonPropertyName("text")]
    public Narrative? Text { get; set; }

    /// <summary>
    /// Contained, inline Resources.
    /// </summary>
    [JsonPropertyName("contained")]
    public List<Resource>? Contained { get; set; }

    /// <summary>
    /// Extension（資源層級）。
    /// </summary>
    [JsonPropertyName("extension")]
    public List<IExtension>? Extension { get; set; }

    /// <summary>
    /// Modifier extensions that cannot be ignored.
    /// </summary>
    [JsonPropertyName("modifierExtension")]
    public List<IExtension>? ModifierExtension { get; set; }

    /// <inheritdoc />
    public override Base DeepCopy()
    {
        var copy = (DomainResource)base.DeepCopy();
        copy.Text = Text?.DeepCopy() as Narrative;
        copy.Contained = Contained?.Select(r => (Resource)r.DeepCopy()).ToList();
        copy.Extension = Extension?.Select(e => (e.DeepCopy() as IExtension)!).ToList();
        copy.ModifierExtension = ModifierExtension?.Select(e => (e.DeepCopy() as IExtension)!).ToList();
        return copy;
    }

    /// <inheritdoc />
    public override bool IsExactly(Base other)
    {
        if (other is not DomainResource o)
            return false;

        if (!base.IsExactly(other))
            return false;

        if (!ValueEq(Text, o.Text))
            return false;

        if (Contained?.Count != o.Contained?.Count)
            return false;
        if (Contained != null && o.Contained != null &&
            !Contained.Zip(o.Contained, (a, b) => a.IsExactly(b)).All(x => x))
            return false;

        if (Extension?.Count != o.Extension?.Count)
            return false;
        if (Extension != null && o.Extension != null &&
            !Extension.Zip(o.Extension, (a, b) => a.IsExactly(b as ITypeFramework)).All(x => x))
            return false;

        if (ModifierExtension?.Count != o.ModifierExtension?.Count)
            return false;
        if (ModifierExtension != null && o.ModifierExtension != null &&
            !ModifierExtension.Zip(o.ModifierExtension, (a, b) => a.IsExactly(b as ITypeFramework)).All(x => x))
            return false;

        return true;
    }

    private static bool ValueEq<T>(T? a, T? b) where T : Base =>
        (a == null && b == null) || (a != null && b != null && a.IsExactly(b));

    /// <inheritdoc />
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var r in base.Validate(validationContext))
            yield return r;

        if (Text != null)
        {
            foreach (var x in Text.Validate(new ValidationContext(Text)))
                yield return x;
        }

        if (Contained != null)
        {
            foreach (var res in Contained)
            {
                foreach (var x in res.Validate(new ValidationContext(res)))
                    yield return x;
            }
        }

        if (Extension != null)
        {
            foreach (var ext in Extension)
            {
                foreach (var x in ext.Validate(new ValidationContext(ext)))
                    yield return x;
            }
        }

        if (ModifierExtension != null)
        {
            foreach (var ext in ModifierExtension)
            {
                if (string.IsNullOrEmpty(ext.Url))
                    yield return new ValidationResult("modifierExtension 必須包含 url");
                foreach (var x in ext.Validate(new ValidationContext(ext)))
                    yield return x;
            }
        }
    }
}
