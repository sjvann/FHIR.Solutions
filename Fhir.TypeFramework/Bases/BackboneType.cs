using Fhir.TypeFramework.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fhir.TypeFramework.Bases;

/// <summary>
/// BackboneType - FHIR R5 架構中的骨幹型別
/// 用於定義複雜的巢狀結構，支援 modifier extensions
/// </summary>
/// <remarks>
/// FHIR R5 BackboneType (Abstract)
/// 主要用於複雜型別的巢狀結構，與 BackboneElement 類似，但屬於 DataType 分支。
/// 支援 modifier extensions，這些擴展即使不被識別也不能被忽略。
/// </remarks>
public abstract class BackboneType : DataType
{
    /// <summary>
    /// Extensions that cannot be ignored even if unrecognized
    /// FHIR Path: BackboneType.modifierExtension
    /// Cardinality: 0..*
    /// </summary>
    [JsonPropertyName("modifierExtension")]
    public IList<IExtension>? ModifierExtension { get; set; }

    /// <summary>
    /// 檢查是否有 modifier extensions
    /// </summary>
    /// <returns>如果存在 modifier extensions 則為 true，否則為 false</returns>
    [JsonIgnore]
    public bool HasModifierExtensions => ModifierExtension?.Any() == true;

    /// <summary>
    /// 取得指定 URL 的 modifier extension
    /// </summary>
    /// <param name="url">要查詢的擴展 URL</param>
    /// <returns>找到的擴展，如果不存在則為 null</returns>
    public IExtension? GetModifierExtension(string url)
    {
        return ModifierExtension?.FirstOrDefault(ext => ext.Url == url);
    }

    /// <summary>
    /// 添加 modifier extension
    /// </summary>
    /// <param name="url">擴展的 URL</param>
    /// <param name="value">擴展的值</param>
    public void AddModifierExtension(string url, object? value)
    {
        ModifierExtension ??= new List<IExtension>();
        var extension = new DataTypes.Extension { Url = url, Value = value };
        ModifierExtension.Add(extension);
    }

    /// <summary>
    /// 移除指定 URL 的 modifier extension
    /// </summary>
    /// <param name="url">要移除的擴展 URL</param>
    /// <returns>如果成功移除則為 true，否則為 false</returns>
    public bool RemoveModifierExtension(string url)
    {
        if (ModifierExtension == null) return false;
        var toRemove = ModifierExtension.Where(ext => ext.Url == url).ToList();
        foreach (var ext in toRemove)
        {
            ModifierExtension.Remove(ext);
        }
        return toRemove.Any();
    }

    /// <summary>
    /// 建立物件的深層複本
    /// </summary>
    /// <returns>BackboneType 的深層複本</returns>
    public override Base DeepCopy()
    {
        var copy = (BackboneType)Activator.CreateInstance(GetType())!;
        copy.Id = Id?.DeepCopy() as FhirString;
        if (Extension != null)
        {
            copy.Extension = Extension.Select(ext => (ext.DeepCopy() as IExtension)!).ToList();
        }
        if (ModifierExtension != null)
        {
            copy.ModifierExtension = ModifierExtension.Select(ext => (ext.DeepCopy() as IExtension)!).ToList();
        }
        return copy;
    }

    /// <summary>
    /// 判斷與另一個 BackboneType 物件是否相等
    /// </summary>
    /// <param name="other">要比較的物件</param>
    /// <returns>如果兩個物件相等則為 true，否則為 false</returns>
    public override bool IsExactly(Base other)
    {
        if (other is not BackboneType otherBackbone)
            return false;
        return base.IsExactly(other)
            && ModifierExtension?.Count == otherBackbone.ModifierExtension?.Count
            && (ModifierExtension?.Zip(otherBackbone.ModifierExtension ?? new List<IExtension>(), (a, b) => a.IsExactly(b as ITypeFramework)).All(x => x) ?? true);
    }

    /// <summary>
    /// 驗證 BackboneType 是否符合 FHIR 規範
    /// </summary>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果集合</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 驗證 modifier extensions
        if (ModifierExtension != null)
        {
            foreach (var ext in ModifierExtension)
            {
                if (string.IsNullOrEmpty(ext.Url))
                {
                    yield return new ValidationResult("ModifierExtension must have a URL");
                }
                var extValidationContext = new ValidationContext(ext);
                foreach (var extResult in ext.Validate(extValidationContext))
                {
                    yield return extResult;
                }
            }
        }
        // 呼叫基礎驗證
        foreach (var result in base.Validate(validationContext))
        {
            yield return result;
        }
    }
} 