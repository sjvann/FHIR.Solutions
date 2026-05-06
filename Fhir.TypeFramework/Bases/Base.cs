using Fhir.TypeFramework.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Fhir.TypeFramework.Bases;

/// <summary>
/// Base definition for all types defined in FHIR type system.
/// 所有 FHIR 型別的最基礎類別
/// </summary>
/// <remarks>
/// FHIR R5 Base (Abstract)
/// This is the root of the FHIR type hierarchy.
/// 提供所有 FHIR 型別的基本功能，包括型別名稱、深層複製、相等性比較和驗證。
/// </remarks>
public abstract class Base : ITypeFramework, IValidatableObject
{
    /// <summary>
    /// 取得型別名稱
    /// </summary>
    /// <returns>型別的名稱</returns>
    public virtual string TypeName => GetType().Name;

    /// <summary>
    /// 建立物件的深層複本
    /// </summary>
    /// <returns>Base 物件的深層複本</returns>
    public abstract Base DeepCopy();

    /// <summary>
    /// 判斷與另一個 Base 物件是否相等
    /// </summary>
    /// <param name="other">要比較的物件</param>
    /// <returns>如果兩個物件相等則為 true，否則為 false</returns>
    public abstract bool IsExactly(Base other);

    /// <summary>
    /// ITypeFramework 實作 - 建立物件的深層複本
    /// </summary>
    /// <returns>ITypeFramework 物件的深層複本</returns>
    ITypeFramework ITypeFramework.DeepCopy() => DeepCopy();

    /// <summary>
    /// ITypeFramework 實作 - 判斷與另一個物件是否相等
    /// </summary>
    /// <param name="other">要比較的 ITypeFramework 物件</param>
    /// <returns>如果兩個物件相等則為 true，否則為 false</returns>
    bool ITypeFramework.IsExactly(ITypeFramework? other) => other is Base baseOther && IsExactly(baseOther);

    /// <summary>
    /// 基礎驗證 - 子類別可以覆寫以提供特定驗證邏輯
    /// </summary>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果集合</returns>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 基礎驗證邏輯可以在這裡實作
        yield break;
    }
}
