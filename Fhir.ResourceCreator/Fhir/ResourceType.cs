using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Fhir
{
    public static class ResourceType
    {
        private static readonly string[] ResourcType = new string[] {"ActivityDefinition", "ActorDefinition",

            "AdministrableProductDefinition",
            "AdverseEvent",
            "AllergyIntolerance",
            "Appointment",
"AppointmentResponse",
"ArtifactAssessment",
"AuditEvent",
"Basic",
"Binary",
"BiologicallyDerivedProductDispense",
"BiologicallyDerivedProduct",
"BodyStructure",
"Bundle",
"CapabilityStatement",
"CarePlan",
"CareTeam",
"ChargeItem",
"ChargeItemDefinition",
"Citation",
"Claim",
"ClaimResponse",
"ClinicalImpression",
"ClinicalUseDefinition",
"CodeSystem",
"CommunicationRequest",
         "Communication",
"CompartmentDefinition",
"Composition",
"ConceptMap",
"ConditionDefinition",
            "Condition",
"Consent",
"Contract",
"Coverage",
"CoverageEligibilityRequest",
"CoverageEligibilityResponse",
"DetectedIssue",
"DeviceAssociation",
"DeviceDefinition",
"DeviceDispense",
"DeviceMetric",
"DeviceRequest",
"DeviceUsage",
            "Device",

"DiagnosticReport",
"DocumentReference",
"EncounterHistory",
            "Encounter",
"Endpoint",
"EnrollmentRequest",
"EnrollmentResponse",
"EpisodeOfCare",
"EventDefinition",
"EvidenceReport",
"EvidenceVariable",
            "Evidence",
"ExampleScenario",
"ExplanationOfBenefit",
"FamilyMemberHistory",
"Flag",
"FormularyItem",
"GenomicStudy",
"Goal",
"GraphDefinition",
"Group",
"GuidanceResponse",
"HealthcareService",
"ImagingSelection",
"ImagingStudy",
"ImmunizationEvaluation",
"ImmunizationRecommendation",
            "Immunization",
"ImplementationGuide",
"Ingredient",
"InsurancePlan",
"InventoryItem",
"InventoryReport",
"Invoice",
"Library",
"Linkage",
"List",
"Location",
"ManufacturedItemDefinition",
"MeasureReport",
            "Measure",
"MedicationAdministration",
"MedicationDispense",
"MedicationKnowledge",
"MedicationRequest",
"MedicationStatement",
"MedicinalProductDefinition",
"Medication",
"MessageDefinition",
"MessageHeader",
"MolecularSequence",
"NamingSystem",
"NutritionIntake",
"NutritionOrder",
"NutritionProduct",
"ObservationDefinition",
"Observation",
"OperationDefinition",
"OperationOutcome",
"OrganizationAffiliation",
"Organization",
"PackagedProductDefinition",
"Parameters",
"Patient",
"PaymentNotice",
"PaymentReconciliation",
"Permission",
"Person",
"PlanDefinition",
"PractitionerRole",
"Practitioner",
"Procedure",
"Provenance",
"QuestionnaireResponse",
"Questionnaire",
"RegulatedAuthorization",
"RelatedPerson",
"RequestOrchestration",
"Requirements",
"ResearchStudy",
"ResearchSubject",
"RiskAssessment",
"Schedule",
"SearchParameter",
"ServiceRequest",
"Slot",

"SpecimenDefinition","Specimen",
"StructureDefinition",
"StructureMap",

"SubscriptionStatus","Subscription",
"SubscriptionTopic",

"SubstanceDefinition","Substance",
"SubstanceNucleicAcid",
"SubstancePolymer",
"SubstanceProtein",
"SubstanceReferenceInformation",
"SubstanceSourceMaterial",
"SupplyDelivery",
"SupplyRequest",
"Task",
"TerminologyCapabilities",
"TestPlan",
"TestReport",
"TestScript",
"Transport",
"ValueSet",
"VerificationResult",
"VisionPrescription"
};
        public static string[] CheckResourceTypesFormString(string resourceString)
        {
            List<string> result = new();
            result.AddRange(CheckResourceType(resourceString));

            return result.ToArray();
        }

        private static List<string> CheckResourceType(string source)
        {
            List<string> result = new();
            string checkTarget = source;
            for (int i = 0; i < ResourcType.Length; i++)
            {
                if (checkTarget.Contains(ResourcType[i]))
                {
                    result.Add(ResourcType[i]);
                    checkTarget = checkTarget.Replace(ResourcType[i], "");
                }
            }
            return result;

        }
    }
}
