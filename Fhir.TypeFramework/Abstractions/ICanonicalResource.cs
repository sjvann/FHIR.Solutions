#nullable enable

using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.Abstractions;

/// <summary>
/// FHIR R5 Canonical Resource 共通欄位（對應規格介面 <c>CanonicalResource</c>；供知識／合規類資源實作）。
/// </summary>
/// <remarks>
/// 涵蓋規格中標準化之中繼資料（含 <c>jurisdiction</c>），實際資源類別（如 StructureDefinition）應實作此介面並填入對應屬性。
/// </remarks>
public interface ICanonicalResource
{
    FhirUri? Url { get; set; }
    List<Identifier>? Identifier { get; set; }
    FhirString? Version { get; set; }
    FhirString? Name { get; set; }
    FhirString? Title { get; set; }
    FhirCode? Status { get; set; }
    FhirBoolean? Experimental { get; set; }
    FhirDateTime? Date { get; set; }
    FhirString? Publisher { get; set; }
    List<ContactDetail>? Contact { get; set; }
    FhirMarkdown? Description { get; set; }
    List<UsageContext>? UseContext { get; set; }
    FhirMarkdown? Purpose { get; set; }
    FhirMarkdown? Copyright { get; set; }
    FhirString? CopyrightLabel { get; set; }
    List<CodeableConcept>? Jurisdiction { get; set; }
}
