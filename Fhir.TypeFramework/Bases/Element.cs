using Fhir.TypeFramework.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Fhir.TypeFramework.Bases;

/// <summary>
/// Base definition for all elements in a resource.
/// 現在可以安全地包含 Extension 而不會有循環依賴
/// </summary>
/// <remarks>
/// FHIR R5 Element (Abstract)
/// 規格階層中為 <c>DataType</c>、<c>BackboneElement</c> 等之上層（見 UML：<c>Base → Element</c>）。
/// 提供 id 與 extension；巢狀 backbone 元素請使用 <see cref="BackboneElement"/>。
/// 
/// Structure:
/// - id: string (0..1) - Unique id for the element within a resource
/// - extension: Extension[] (0..*) - Additional information that is not part of the basic definition
/// </remarks>
public abstract class Element : Base, IIdentifiableTypeFramework, IExtensibleTypeFramework
{
    /// <summary>
    /// Unique id for the element within a resource (for internal references).
    /// FHIR Path: Element.id
    /// Cardinality: 0..1
    /// Type: string
    /// </summary>
    [JsonPropertyName("id")]
    public FhirString? Id { get; set; }

    /// <summary>
    /// Additional content defined by implementations
    /// FHIR Path: Element.extension
    /// Cardinality: 0..*
    /// Type: Extension
    /// </summary>
    [JsonPropertyName("extension")]
    public List<IExtension>? Extension { get; set; }

    /// <summary>
    /// 建立物件的深層複本
    /// </summary>
    /// <returns>Element 的深層複本</returns>
    public override Base DeepCopy()
    {
        var copy = (Element)MemberwiseClone();
        copy.Id = Id?.DeepCopy() as FhirString;

        if (Extension != null)
        {
            copy.Extension = Extension.Select(ext => (ext.DeepCopy() as IExtension)!).ToList();
        }

        return copy;
    }

    /// <summary>
    /// 判斷與另一個物件是否相等
    /// </summary>
    /// <param name="other">要比較的物件</param>
    /// <returns>如果兩個物件相等則為 true，否則為 false</returns>
    public override bool IsExactly(Base other)
    {
        if (other is not Element otherElement)
            return false;

        // 檢查 Id
        if (!((Id == null && otherElement.Id == null) ||
              (Id != null && otherElement.Id != null && Id.IsExactly(otherElement.Id))))
            return false;

        // 檢查 Extension
        if (Extension == null && otherElement.Extension == null)
            return true;

        if (Extension == null || otherElement.Extension == null)
            return false;

        if (Extension.Count != otherElement.Extension.Count)
            return false;

        for (int i = 0; i < Extension.Count; i++)
        {
            if (!Extension[i].IsExactly(otherElement.Extension[i] as ITypeFramework))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 驗證 Element 是否符合 FHIR 規範
    /// </summary>
    /// <param name="validationContext">驗證上下文</param>
    /// <returns>驗證結果集合</returns>
    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 驗證 Id
        if (Id != null)
        {
            foreach (var result in Id.Validate(validationContext))
            {
                yield return result;
            }
        }

        // 驗證 Extension
        if (Extension != null)
        {
            foreach (var ext in Extension)
            {
                var extCtx = new ValidationContext(ext);
                foreach (var result in ext.Validate(extCtx))
                {
                    yield return result;
                }
            }
        }
    }

    public virtual JsonNode? GetJsonNode() => null;
}
