using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>FHIR R5 Datatype Attachment。</summary>
public class Attachment : ComplexTypeBase
{
    [JsonPropertyName("contentType")] public FhirString? ContentType { get; set; }
    [JsonPropertyName("language")] public FhirCode? Language { get; set; }
    [JsonPropertyName("data")] public FhirBase64Binary? Data { get; set; }
    [JsonPropertyName("url")] public FhirUrl? Url { get; set; }
    [JsonPropertyName("size")] public FhirInteger64? Size { get; set; }
    [JsonPropertyName("hash")] public FhirBase64Binary? Hash { get; set; }
    [JsonPropertyName("title")] public FhirString? Title { get; set; }
    [JsonPropertyName("creation")] public FhirDateTime? Creation { get; set; }
    [JsonPropertyName("height")] public FhirPositiveInt? Height { get; set; }
    [JsonPropertyName("width")] public FhirPositiveInt? Width { get; set; }
    [JsonPropertyName("frames")] public FhirPositiveInt? Frames { get; set; }
    [JsonPropertyName("duration")] public FhirDecimal? Duration { get; set; }
    [JsonPropertyName("pages")] public FhirPositiveInt? Pages { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var c = (Attachment)copy;
        c.ContentType = ContentType?.DeepCopy() as FhirString;
        c.Language = Language?.DeepCopy() as FhirCode;
        c.Data = Data?.DeepCopy() as FhirBase64Binary;
        c.Url = Url?.DeepCopy() as FhirUrl;
        c.Size = Size?.DeepCopy() as FhirInteger64;
        c.Hash = Hash?.DeepCopy() as FhirBase64Binary;
        c.Title = Title?.DeepCopy() as FhirString;
        c.Creation = Creation?.DeepCopy() as FhirDateTime;
        c.Height = Height?.DeepCopy() as FhirPositiveInt;
        c.Width = Width?.DeepCopy() as FhirPositiveInt;
        c.Frames = Frames?.DeepCopy() as FhirPositiveInt;
        c.Duration = Duration?.DeepCopy() as FhirDecimal;
        c.Pages = Pages?.DeepCopy() as FhirPositiveInt;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var o = (Attachment)other;
        return ValueEquals(ContentType, o.ContentType)
               && ValueEquals(Language, o.Language)
               && ValueEquals(Data, o.Data)
               && ValueEquals(Url, o.Url)
               && ValueEquals(Size, o.Size)
               && ValueEquals(Hash, o.Hash)
               && ValueEquals(Title, o.Title)
               && ValueEquals(Creation, o.Creation)
               && ValueEquals(Height, o.Height)
               && ValueEquals(Width, o.Width)
               && ValueEquals(Frames, o.Frames)
               && ValueEquals(Duration, o.Duration)
               && ValueEquals(Pages, o.Pages);
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        foreach (var r in ValidateItem(ContentType, validationContext)) yield return r;
        foreach (var r in ValidateItem(Language, validationContext)) yield return r;
        foreach (var r in ValidateItem(Data, validationContext)) yield return r;
        foreach (var r in ValidateItem(Url, validationContext)) yield return r;
        foreach (var r in ValidateItem(Size, validationContext)) yield return r;
        foreach (var r in ValidateItem(Hash, validationContext)) yield return r;
        foreach (var r in ValidateItem(Title, validationContext)) yield return r;
        foreach (var r in ValidateItem(Creation, validationContext)) yield return r;
        foreach (var r in ValidateItem(Height, validationContext)) yield return r;
        foreach (var r in ValidateItem(Width, validationContext)) yield return r;
        foreach (var r in ValidateItem(Frames, validationContext)) yield return r;
        foreach (var r in ValidateItem(Duration, validationContext)) yield return r;
        foreach (var r in ValidateItem(Pages, validationContext)) yield return r;
    }
}
