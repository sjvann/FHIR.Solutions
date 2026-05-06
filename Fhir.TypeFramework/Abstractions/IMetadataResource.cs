#nullable enable

using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.Abstractions;

/// <summary>
/// FHIR R5 Metadata Resource 共通欄位（對應規格介面 <c>MetadataResource</c>）。
/// </summary>
/// <remarks>
/// 含知識資產相關作者／審查者與 <c>topic</c> 等；具體資源應實作此介面。
/// </remarks>
public interface IMetadataResource
{
    FhirDate? ApprovalDate { get; set; }
    FhirDate? LastReviewDate { get; set; }
    Period? EffectivePeriod { get; set; }
    List<Contributor>? Author { get; set; }
    List<Contributor>? Editor { get; set; }
    List<Contributor>? Reviewer { get; set; }
    List<Contributor>? Endorser { get; set; }
    List<RelatedArtifact>? RelatedArtifact { get; set; }
    List<CodeableConcept>? Topic { get; set; }
}
