using Fhir.Terminology.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fhir.Terminology.Infrastructure;

/// <summary>
/// HL7 FHIR 規格所列常見<strong>外部編碼系統</strong>（terminology.hl7.org「External Code systems」），
/// 以最小 CodeSystem（<c>content=not-present</c>）登錄於本機，供 <c>$lookup</c> 路由與 TerminologyCapabilities／SEARCH 辨識 canonical URI；
/// <strong>不含</strong>完整詞表（請搭配 upstream 或自行匯入）。
/// </summary>
public static class ExternalCanonicalCodeSystemsSeed
{
    /// <summary>Logical Id → FHIR JSON（<c>resourceType</c>、<c>id</c> 已對齊）。</summary>
    private static readonly (string LogicalId, string Json)[] Entries =
    [
        ("canonical-snomed-ct", """
            {"resourceType":"CodeSystem","id":"canonical-snomed-ct","url":"http://snomed.info/sct","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.96"}],"version":"0","name":"SNOMEDCT","title":"SNOMED CT","status":"active","description":"HL7 FHIR external code system (SNOMED CT). OID 2.16.840.1.113883.6.96. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-rxnorm", """
            {"resourceType":"CodeSystem","id":"canonical-rxnorm","url":"http://www.nlm.nih.gov/research/umls/rxnorm","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.88"}],"version":"0","name":"RxNorm","title":"RxNorm","status":"active","description":"HL7 FHIR external code system (RxNorm, US NLM). OID 2.16.840.1.113883.6.88. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-loinc", """
            {"resourceType":"CodeSystem","id":"canonical-loinc","url":"http://loinc.org","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.1"}],"version":"0","name":"LOINC","title":"LOINC","status":"active","description":"HL7 FHIR external code system (LOINC). OID 2.16.840.1.113883.6.1. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-pclocd", """
            {"resourceType":"CodeSystem","id":"canonical-pclocd","url":"https://fhir.infoway-inforoute.ca/CodeSystem/pCLOCD","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.2.20.5.1"}],"version":"0","name":"PCLOCD","title":"pCLOCD","status":"active","description":"HL7 FHIR external code system (pCLOCD, Canada Infoway). OID 2.16.840.1.113883.2.20.5.1. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-ucum", """
            {"resourceType":"CodeSystem","id":"canonical-ucum","url":"http://unitsofmeasure.org","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.8"}],"version":"0","name":"UCUM","title":"UCUM","status":"active","description":"HL7 FHIR external code system (UCUM; case-sensitive). OID 2.16.840.1.113883.6.8. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-cpt", """
            {"resourceType":"CodeSystem","id":"canonical-cpt","url":"http://www.ama-assn.org/go/cpt","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.12"}],"version":"0","name":"CPT","title":"AMA CPT","status":"active","description":"HL7 FHIR external code system (AMA CPT). OID 2.16.840.1.113883.6.12. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-medrt", """
            {"resourceType":"CodeSystem","id":"canonical-medrt","url":"http://va.gov/terminology/medrt","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.345"}],"version":"0","name":"MEDRT","title":"MED-RT","status":"active","description":"HL7 FHIR external code system (MED-RT). OID 2.16.840.1.113883.6.345. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-unii", """
            {"resourceType":"CodeSystem","id":"canonical-unii","url":"http://fdasis.nlm.nih.gov","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.4.9"}],"version":"0","name":"UNII","title":"UNII (Unique Ingredient Identifier)","status":"active","description":"HL7 FHIR external code system (UNII). OID 2.16.840.1.113883.4.9. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-ndc", """
            {"resourceType":"CodeSystem","id":"canonical-ndc","url":"http://hl7.org/fhir/sid/ndc","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.69"}],"version":"0","name":"NDC","title":"NDC / NHRIC","status":"active","description":"HL7 FHIR SID (NDC/NHRIC). OID 2.16.840.1.113883.6.69. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-cvx", """
            {"resourceType":"CodeSystem","id":"canonical-cvx","url":"http://hl7.org/fhir/sid/cvx","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.12.292"}],"version":"0","name":"CVX","title":"CVX (Vaccine Administered)","status":"active","description":"HL7 FHIR SID (CVX). OID 2.16.840.1.113883.12.292. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-iso3166", """
            {"resourceType":"CodeSystem","id":"canonical-iso3166","url":"urn:iso:std:iso:3166","version":"0","name":"ISO3166","title":"ISO 3166 (country & regional codes)","status":"active","description":"HL7 FHIR external code system (ISO 3166). OID n/a in FHIR table. Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-iso3166-2", """
            {"resourceType":"CodeSystem","id":"canonical-iso3166-2","url":"urn:iso:std:iso:3166:-2","version":"0","name":"ISO3166Part2","title":"ISO 3166-2","status":"active","description":"HL7 FHIR external code system (ISO 3166-2 subdivision codes). Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-iso3166-3", """
            {"resourceType":"CodeSystem","id":"canonical-iso3166-3","url":"urn:iso:std:iso:3166:-3","version":"0","name":"ISO3166Part3","title":"ISO 3166-3","status":"active","description":"HL7 FHIR external code system (ISO 3166-3 former country codes). Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-icd10-hl7-sid", """
            {"resourceType":"CodeSystem","id":"canonical-icd10-hl7-sid","url":"http://hl7.org/fhir/sid/icd-10","version":"0","name":"ICD10FHIRSID","title":"ICD-10 (HL7 FHIR SID)","status":"active","description":"HL7 FHIR SID for ICD-10 family (see Using ICD with HL7 for national variants). Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-icd9cm-hl7-sid", """
            {"resourceType":"CodeSystem","id":"canonical-icd9cm-hl7-sid","url":"http://hl7.org/fhir/sid/icd-9-cm","version":"0","name":"ICD9CMFHIRSID","title":"ICD-9-CM (HL7 FHIR SID)","status":"active","description":"HL7 FHIR SID for ICD-9-CM (US). Full content not stored locally.","content":"not-present"}
            """),
        ("canonical-iso11073-10101", """
            {"resourceType":"CodeSystem","id":"canonical-iso11073-10101","url":"urn:iso:std:iso:11073:10101","identifier":[{"system":"urn:ietf:rfc:3986","value":"urn:oid:2.16.840.1.113883.6.24"}],"version":"0","name":"ISO1107310101","title":"ISO 11073-10101 (MDC)","status":"active","description":"HL7 FHIR external code system (Medical Device Codes / ISO 11073-10101). OID 2.16.840.1.113883.6.24. Full content not stored locally.","content":"not-present"}
            """),
    ];

    /// <summary>
    /// 若指定 logical id 尚不存在則建立；已存在則略過（可安全於每次啟動呼叫）。
    /// </summary>
    public static async Task EnsureRegisteredAsync(
        TerminologyDbContext db,
        ITerminologyRepository repository,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var inserted = 0;
        foreach (var (logicalId, json) in Entries)
        {
            var exists = await db.TerminologyResources.AsNoTracking()
                .AnyAsync(x => x.ResourceType == "CodeSystem" && x.LogicalId == logicalId, cancellationToken);
            if (exists)
                continue;

            await repository.CreateAsync(json, cancellationToken);
            inserted++;
        }

        if (inserted > 0)
            logger.LogInformation("Registered {Inserted} HL7 external canonical CodeSystem stubs (content=not-present).", inserted);
    }
}
