using System.Globalization;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>
/// FHIR R5 <c>date</c> primitive（年、年-月、年-月-日等部分精度之字串與 <see cref="DateTime"/> 投影）。
/// </summary>
/// <remarks>
/// 與其他系統交換或儲存時，<see cref="Bases.PrimitiveType{DateTime}.StringValue"/> 為 FHIR 規範之 lexical 字串，可完整表達 gYear／gYearMonth／date。
/// 僅在需要曆法運算時使用 <see cref="Bases.PrimitiveType{DateTime}.Value"/>；由 <see cref="DateTime"/> 寫回時，字串格式為不變文化之 <c>yyyy-MM-dd</c> 完整日曆日，無法還原「僅年」等較低精度。
/// 代表樣本可參考 <see cref="FhirTemporalLexicalExamples.FhirDateLexical"/>。
/// </remarks>
public class FhirDate : DateTimePrimitiveTypeBase<DateTime>, IDateTimeValue
{
    public FhirDate() { }
    public FhirDate(DateTime v) : base(v) { }
    public FhirDate(string? v) : base(v) { }

    public static implicit operator FhirDate?(DateTime? value) => CreateFromDateTime<FhirDate>(value);
    public static implicit operator DateTime?(FhirDate? instance) => GetDateTimeValue(instance);
    public static implicit operator FhirDate?(string? value) => value is null ? null : new FhirDate(value);
    public static implicit operator string?(FhirDate? instance) => instance?.StringValue;

    /// <summary>自 <see cref="DateTime"/> 寫回字串時，採不變文化之公曆 <c>yyyy-MM-dd</c>，與執行緒文化無關。</summary>
    protected override string? ConvertToStringValue(DateTime value)
        => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    protected override bool ValidateDateTimeValue(DateTime value) => true;

    DateTime? IValue<DateTime?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

