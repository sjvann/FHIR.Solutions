using System.Globalization;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>
/// FHIR R5 <c>dateTime</c> primitive（date 與 time 之 union 的 lexical 與 <see cref="DateTime"/> 投影）。
/// </summary>
/// <remarks>
/// 與交換層一致之字串請以 <see cref="Bases.PrimitiveType{DateTime}.StringValue"/> 為準；<see cref="Bases.PrimitiveType{DateTime}.Value"/> 供曆法／時區轉換等情境使用，寫回時採 ISO 8601 延伸格式（<c>O</c>）與執行緒文化無關。
/// 代表樣本可參考 <see cref="FhirTemporalLexicalExamples.FhirDateTimeLexical"/>。
/// </remarks>
public class FhirDateTime : DateTimePrimitiveTypeBase<DateTime>, IDateTimeValue
{
    public FhirDateTime() { }
    public FhirDateTime(DateTime v) : base(v) { }
    public FhirDateTime(string? v) : base(v) { }

    public static implicit operator FhirDateTime?(DateTime? value) => CreateFromDateTime<FhirDateTime>(value);
    public static implicit operator DateTime?(FhirDateTime? instance) => GetDateTimeValue(instance);
    public static implicit operator FhirDateTime?(string? value) => value is null ? null : new FhirDateTime(value);
    public static implicit operator string?(FhirDateTime? instance) => instance?.StringValue;

    /// <summary>自 <see cref="DateTime"/> 寫回時使用 ISO 8601 延伸格式，避免依使用者文化格式化。</summary>
    protected override string? ConvertToStringValue(DateTime value)
        => value.ToString("O", CultureInfo.InvariantCulture);

    protected override bool ValidateDateTimeValue(DateTime value) => true;

    DateTime? IValue<DateTime?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

