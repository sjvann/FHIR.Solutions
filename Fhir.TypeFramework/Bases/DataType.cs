using System.ComponentModel.DataAnnotations;

namespace Fhir.TypeFramework.Bases;

/// <summary>
/// DataType - 資料型別基礎類別
/// 所有 FHIR 資料型別的基礎
/// </summary>
/// <remarks>
/// FHIR R5 DataType (Abstract)
/// Base definition for all data types in FHIR.
/// DataType 繼承自 Element，因此具有 id 和 extension 屬性
/// </remarks>
public abstract class DataType : Element
{
    /// <summary>
    /// 判斷與另一個 DataType 物件是否相等
    /// </summary>
    /// <param name="other">要比較的物件</param>
    /// <returns>如果兩個物件相等則為 true，否則為 false</returns>
    public override bool IsExactly(Base other)
    {
        if (other is not DataType otherDataType)
            return false;

        // 先檢查 Element 的相等性（包括 id 和 extension）
        if (!base.IsExactly(other))
            return false;

        // DataType 特有的相等性檢查可以在子類別中實作
        return true;
    }

    /// <summary>
    /// 驗證 DataType 是否符合 FHIR 規範
    /// </summary>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果集合</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 呼叫 Element 的驗證（包括 id 和 extension 驗證）
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }

        // DataType 特有的驗證可以在子類別中實作
    }
} 