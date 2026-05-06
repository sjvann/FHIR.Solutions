using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.Serialization;

namespace Fhir.TypeFramework.Tests.Bases;

/// <summary>
/// FHIR date／dateTime／time／instant 之 lexical 字串應優先保持與交換層一致；此處驗證 StringValue、JSON、DeepCopy 之 round-trip。
/// </summary>
public sealed class FhirTemporalRoundTripTests
{
    public static TheoryData<string> FhirDateCases()
    {
        var d = new TheoryData<string>();
        foreach (var s in FhirTemporalLexicalExamples.FhirDateLexical)
            d.Add(s);
        return d;
    }

    public static TheoryData<string> FhirDateTimeCases()
    {
        var d = new TheoryData<string>();
        foreach (var s in FhirTemporalLexicalExamples.FhirDateTimeLexical)
            d.Add(s);
        return d;
    }

    public static TheoryData<string> FhirInstantCases()
    {
        var d = new TheoryData<string>();
        foreach (var s in FhirTemporalLexicalExamples.FhirInstantLexical)
            d.Add(s);
        return d;
    }

    public static TheoryData<string> FhirTimeCases()
    {
        var d = new TheoryData<string>();
        foreach (var s in FhirTemporalLexicalExamples.FhirTimeLexical)
            d.Add(s);
        return d;
    }

    [Theory]
    [MemberData(nameof(FhirDateCases))]
    public void FhirDate_StringValue_RoundTrips_Through_Json_And_DeepCopy(string lexical)
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var d = new FhirDate(lexical);
            Assert.Equal(lexical, d.StringValue);

            var jv = d.ToJsonValue();
            Assert.NotNull(jv);
            var d2 = new FhirDate();
            d2.FromJsonValue(jv);
            Assert.Equal(lexical, d2.StringValue);

            var full = d.ToFullJsonObject();
            Assert.NotNull(full);
            var d3 = new FhirDate();
            d3.FromFullJsonObject(full);
            Assert.Equal(lexical, d3.StringValue);

            var copy = (FhirDate)d.DeepCopy();
            Assert.Equal(lexical, copy.StringValue);
            Assert.True(d.IsExactly(copy));

            var json = FhirJsonSerializer.Serialize(d);
            var round = FhirJsonSerializer.Deserialize<FhirDate>(json);
            Assert.NotNull(round);
            Assert.Equal(lexical, round!.StringValue);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Theory]
    [MemberData(nameof(FhirDateTimeCases))]
    public void FhirDateTime_StringValue_RoundTrips_Through_Json_And_DeepCopy(string lexical)
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var d = new FhirDateTime(lexical);
            Assert.Equal(lexical, d.StringValue);

            var jv = d.ToJsonValue();
            var d2 = new FhirDateTime();
            d2.FromJsonValue(jv);
            Assert.Equal(lexical, d2.StringValue);

            var full = d.ToFullJsonObject();
            var d3 = new FhirDateTime();
            d3.FromFullJsonObject(full);
            Assert.Equal(lexical, d3.StringValue);

            var copy = (FhirDateTime)d.DeepCopy();
            Assert.Equal(lexical, copy.StringValue);
            Assert.True(d.IsExactly(copy));

            var json = FhirJsonSerializer.Serialize(d);
            var round = FhirJsonSerializer.Deserialize<FhirDateTime>(json);
            Assert.NotNull(round);
            Assert.Equal(lexical, round!.StringValue);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Theory]
    [MemberData(nameof(FhirInstantCases))]
    public void FhirInstant_StringValue_RoundTrips_Through_Json_And_DeepCopy(string lexical)
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var d = new FhirInstant(lexical);
            Assert.Equal(lexical, d.StringValue);

            var jv = d.ToJsonValue();
            var d2 = new FhirInstant();
            d2.FromJsonValue(jv);
            Assert.Equal(lexical, d2.StringValue);

            var copy = (FhirInstant)d.DeepCopy();
            Assert.Equal(lexical, copy.StringValue);
            Assert.True(d.IsExactly(copy));

            var json = FhirJsonSerializer.Serialize(d);
            var round = FhirJsonSerializer.Deserialize<FhirInstant>(json);
            Assert.NotNull(round);
            Assert.Equal(lexical, round!.StringValue);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Theory]
    [MemberData(nameof(FhirTimeCases))]
    public void FhirTime_StringValue_RoundTrips_Through_Json_And_DeepCopy(string lexical)
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            var t = new FhirTime(lexical);
            Assert.Equal(lexical, t.StringValue);

            var jv = t.ToJsonValue();
            var t2 = new FhirTime();
            t2.FromJsonValue(jv);
            Assert.Equal(lexical, t2.StringValue);

            var copy = (FhirTime)t.DeepCopy();
            Assert.Equal(lexical, copy.StringValue);
            Assert.True(t.IsExactly(copy));

            var json = FhirJsonSerializer.Serialize(t);
            var round = FhirJsonSerializer.Deserialize<FhirTime>(json);
            Assert.NotNull(round);
            Assert.Equal(lexical, round!.StringValue);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }

    [Fact]
    public void Documentation_PartialPrecision_Prefer_StringValue_Not_DateTime_RoundTrip()
    {
        var prev = PrimitiveTypeOptions.TypedParseTiming;
        try
        {
            PrimitiveTypeOptions.TypedParseTiming = PrimitiveTypedParseTiming.Deferred;
            // 僅「年」之 lexical：StringValue 永遠保留交換字串；CLR 未必能將其解析為 DateTime（視版本／格式而定）。
            var yearOnly = new FhirDate("1997");
            Assert.Equal("1997", yearOnly.StringValue);

            // 完整日曆日：透過 Value 往返後，字串仍以 ISO 日曆分量表示（與 StringValue 一致）。
            var fullDay = new FhirDate("1997-10-24");
            var lexicalBefore = fullDay.StringValue;
            fullDay.Value = fullDay.Value;
            Assert.Equal(lexicalBefore, fullDay.StringValue);

            // 若將完整 DateTime 指派給「僅年」之欄位，會覆寫 lexical（開發者宜直接操作 StringValue 以保留部分精度）。
            yearOnly.Value = new DateTime(1997, 6, 15, 0, 0, 0, DateTimeKind.Unspecified);
            Assert.Equal("1997-06-15", yearOnly.StringValue);
        }
        finally
        {
            PrimitiveTypeOptions.TypedParseTiming = prev;
        }
    }
}
