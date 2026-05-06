using System.Globalization;
using Fhir.TypeFramework.Bases;

namespace Fhir.TypeFramework.DataTypes;

/// <summary>
/// FHIR R5 <c>time</c> primitive（一日內之時刻，字串與 <see cref="TimeSpan"/> 投影）。
/// </summary>
/// <remarks>
/// 交換字串請以 <see cref="Bases.PrimitiveType{TimeSpan}.StringValue"/> 為準；由 <see cref="TimeSpan"/> 寫回時為不變文化之固定格式（見 <see cref="ConvertToStringValue"/>）。
/// 代表樣本可參考 <see cref="FhirTemporalLexicalExamples.FhirTimeLexical"/>。
/// </remarks>
public class FhirTime : DateTimePrimitiveTypeBase<TimeSpan>
{
    public FhirTime() { }
    public FhirTime(TimeSpan v) : base(v) { }
    public FhirTime(string? v) : base(v) { }

    public static implicit operator FhirTime?(TimeSpan? value) => CreateFromDateTime<FhirTime>(value);
    public static implicit operator TimeSpan?(FhirTime? instance) => GetDateTimeValue(instance);
    public static implicit operator FhirTime?(string? value) => value is null ? null : new FhirTime(value);
    public static implicit operator string?(FhirTime? instance) => instance?.StringValue;

    /// <summary>將一日內之時刻格式化為與文化無關之字串（必要時含小數秒）。</summary>
    protected override string? ConvertToStringValue(TimeSpan value)
    {
        var t = value;
        if (t < TimeSpan.Zero)
            t = TimeSpan.Zero;
        if (t >= TimeSpan.FromDays(1))
            t = TimeSpan.FromTicks(t.Ticks % TimeSpan.TicksPerDay);
        var s = t.ToString(@"hh\:mm\:ss\.fffffff", CultureInfo.InvariantCulture);
        return s.TrimEnd('0').TrimEnd('.');
    }

    protected override bool ValidateDateTimeValue(TimeSpan value) => true;
}

