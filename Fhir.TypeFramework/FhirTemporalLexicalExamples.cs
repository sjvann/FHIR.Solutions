using System.Collections.ObjectModel;

namespace Fhir.TypeFramework;

/// <summary>
/// FHIR R5 規格中 <c>date</c>、<c>dateTime</c>、<c>time</c>、<c>instant</c> 常見的「字串 lexical」代表樣本（含部分精度）。
/// </summary>
/// <remarks>
/// <para>
/// 與伺服器或 JSON 交換時，規範表示為字串；本類別僅整理開發時常用的合法範例，方便 IntelliSense 對照與單元測試資料來源，
/// 不等同於完整 FHIR 正規表示式驗證。
/// </para>
/// <para>
/// 若需精確保留「僅年」「僅年-月」等精度，請優先讀寫各 primitive 的 StringValue（JSON 的 value 欄位）；
/// <see cref="System.DateTime"/> / <see cref="System.TimeSpan"/> 屬性為便利投影，可能無法承載部分精度。
/// </para>
/// </remarks>
public static class FhirTemporalLexicalExamples
{
    /// <summary>對應 FHIR <c>date</c>：年、年-月、完整日期的常見 lexical。</summary>
    public static readonly IReadOnlyList<string> FhirDateLexical = new ReadOnlyCollection<string>(new[]
    {
        "1997",
        "1997-10",
        "1997-10-24",
    });

    /// <summary>對應 FHIR <c>dateTime</c>：日期、具時區或 UTC 的常見 lexical（規格為 union，此處僅列代表樣本）。</summary>
    public static readonly IReadOnlyList<string> FhirDateTimeLexical = new ReadOnlyCollection<string>(new[]
    {
        "1997-10-24",
        "1997-10-24T12:30:00",
        "1997-10-24T12:30:00+01:00",
        "1997-10-24T12:30:00Z",
        "1997-01", // 規格允許之部分精度（與 .NET 解析相容時可儲存）
    });

    /// <summary>對應 FHIR <c>instant</c>：須含時區資訊之 xs:dateTime 常見樣本。</summary>
    public static readonly IReadOnlyList<string> FhirInstantLexical = new ReadOnlyCollection<string>(new[]
    {
        "2015-02-07T13:28:17.239+02:00",
        "2015-02-07T13:28:17Z",
        "2015-02-07T13:28:17.239Z",
    });

    /// <summary>對應 FHIR <c>time</c>：時分秒與小數秒常見樣本。</summary>
    public static readonly IReadOnlyList<string> FhirTimeLexical = new ReadOnlyCollection<string>(new[]
    {
        "12:30:00",
        "14:30:00.5",
        "23:59:59",
    });
}
