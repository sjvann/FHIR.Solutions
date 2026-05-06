using Fhir.TypeFramework.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fhir.TypeFramework.Bases;

/// <summary>
/// BackboneElement - 骨幹元素
/// 用於定義複雜的巢狀結構
/// </summary>
/// <remarks>
/// FHIR R5 BackboneElement (Abstract)
/// Base definition for all elements that are defined inside a resource - but not those in a data type.
/// 支援 modifier extensions，這些擴展可以修改元素的含義。
/// </remarks>
public abstract class BackboneElement : Element
{
    /// <inheritdoc />
    public override Base DeepCopy()
    {
        var copy = (BackboneElement)base.DeepCopy();
        if (ModifierExtension != null)
        {
            copy.ModifierExtension = ModifierExtension.Select(ext => (ext.DeepCopy() as IExtension)!).ToList();
        }

        return copy;
    }

    /// <summary>
    /// May be used to represent additional information that is not part of the basic definition of the element and that modifies the understanding of the element in which it is contained and/or the understanding of the containing element's descendants.
    /// FHIR Path: BackboneElement.modifierExtension
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
    /// 判斷與另一個 BackboneElement 物件是否相等
    /// </summary>
    /// <param name="other">要比較的物件</param>
    /// <returns>如果兩個物件相等則為 true，否則為 false</returns>
    public override bool IsExactly(Base other)
    {
        if (other is not BackboneElement otherBackbone)
            return false;

        return base.IsExactly(other) &&
               ModifierExtension?.Count == otherBackbone.ModifierExtension?.Count &&
               (ModifierExtension?.Zip(otherBackbone.ModifierExtension ?? new List<IExtension>(), 
                                    (a, b) => a.IsExactly(b as ITypeFramework)).All(x => x) ?? true);
    }

    /// <summary>
    /// 驗證 BackboneElement 是否符合 FHIR 規範
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
                
                // 驗證 ModifierExtension 本身
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