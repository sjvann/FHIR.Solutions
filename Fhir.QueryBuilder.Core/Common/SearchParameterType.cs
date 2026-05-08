namespace Fhir.QueryBuilder.Common;

/// <summary>FHIR 搜尋參數型別（對應 CapabilityStatement 之宣告）。</summary>
public enum SearchParameterType
{
    Number,
    Date,
    String,
    Token,
    Reference,
    Composite,
    Quantity,
    Uri,
    Special,
}
