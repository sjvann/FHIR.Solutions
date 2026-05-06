using System.Globalization;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>
/// FHIR R5 <c>instant</c> primitive（必含時區資訊之時間點）。
/// </summary>
/// <remarks>
/// 交換字串仍以 <see cref="Bases.PrimitiveType{DateTime}.StringValue"/> 為準；由 <see cref="DateTime"/> 寫回時，UTC 以 <c>Z</c> 結尾，其餘以 ISO 8601 延伸格式輸出。
/// 代表樣本可參考 <see cref="FhirTemporalLexicalExamples.FhirInstantLexical"/>。
/// </remarks>
public class FhirInstant : DateTimePrimitiveTypeBase<DateTime>, IDateTimeValue
{
    public FhirInstant() { }
    public FhirInstant(DateTime v) : base(v) { }
    public FhirInstant(string? v) : base(v) { }

    public static implicit operator FhirInstant?(DateTime? value) => CreateFromDateTime<FhirInstant>(value);
    public static implicit operator DateTime?(FhirInstant? instance) => GetDateTimeValue(instance);
    public static implicit operator FhirInstant?(string? value) => value is null ? null : new FhirInstant(value);
    public static implicit operator string?(FhirInstant? instance) => instance?.StringValue;

    protected override string? ConvertToStringValue(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
            return value.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
        return value.ToString("O", CultureInfo.InvariantCulture);
    }

    protected override bool ValidateDateTimeValue(DateTime value) => true;

    DateTime? IValue<DateTime?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

