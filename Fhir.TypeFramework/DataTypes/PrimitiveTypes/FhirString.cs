using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes.PrimitiveTypes;

/// <summary>
/// FHIR String primitive type
/// A sequence of Unicode characters
/// </summary>
/// <remarks>
/// FHIR R5 String
/// Note that FHIR strings SHALL NOT exceed 1MB in size
///
/// 使用泛型設計：PrimitiveType&lt;string&gt; 並實作 IStringValue 介面
/// </remarks>
public class FhirString : StringPrimitiveTypeBase, IStringValue
{
    #region 建構函式

    /// <summary>
    /// 預設建構函式
    /// </summary>
    public FhirString() : base() { }

    /// <summary>
    /// 使用值初始化的建構函式
    /// </summary>
    /// <param name="value">初始值</param>
    public FhirString(string? value) : base(value) { }

    #endregion

    #region IStringValue 實作

    /// <summary>
    /// 實作 IStringValue.Value 屬性
    /// </summary>
    string? IValue<string>.Value => Value;

    #endregion

    #region 隱式轉換運算子

    /// <summary>
    /// 從 string 隱式轉換為 FhirString
    /// </summary>
    /// <param name="value">要轉換的值</param>
    /// <returns>FhirString 實例</returns>
    public static implicit operator FhirString?(string? value)
    {
        return value == null ? null : new FhirString(value);
    }

    /// <summary>
    /// 從 FhirString 隱式轉換為 string
    /// </summary>
    /// <param name="fhirString">要轉換的 FhirString</param>
    /// <returns>string 值</returns>
    public static implicit operator string?(FhirString? fhirString)
    {
        return fhirString?.StringValue;
    }

    #endregion

    protected override bool ValidateStringValue(string value) => true;
}
