using FhirResourceCreator.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Fhir
{
    public static class DataTypes
    {
        private static readonly string[] PrimitiveType = new string[] { "base64Binary", "boolean", "canonical", "code", "dateTime", "date", "decimal", "uuid", "oid", "id", "instant", "integer64", "integer", "markdown", "string", "positiveInt", "time", "unsignedInt", "uri", "url" };
        private static readonly string[] ComplexType = new string[] { "Address", "Age", "Annotation", "Attachment", "CodeableConcept", "CodeableReference", "Coding", "ContactPoint", "Count", "Distance", "Duration", "HumanName", "Identifier", "Money", "Period", "Quantity", "Range", "Ratio", "RatioRange", "Reference", "SampledData", "Signature", "Timing", "ContactDetail", "DataRequirement", "Expression", "ParameterDefinition", "RelatedArtifact", "TriggerDefinition", "UsageContext", "Availability", "ExtendedContactDetail", "Dosage", "Meta" };

        public static string[] CheckDataTypesFormString(string dataTypeString)
        {
            List<string> result = new List<string>();
            
            result.AddRange(CheckDataType(dataTypeString.RemoveSomeString('(',')')));

            return result.ToArray();
        }



        private static List<string> CheckDataType(string source)
        {
            List<string> result = new List<string>();
            string checkTarget = source;
            for (int i = 0; i < ComplexType.Length; i++)
            {
                if (checkTarget.Contains(ComplexType[i]))
                {
                    result.Add(ComplexType[i]);
                    checkTarget = checkTarget.Replace(ComplexType[i], "");
                }
            }
            for (int i = 0; i < PrimitiveType.Length; i++)
            {
                if (checkTarget.Contains(PrimitiveType[i]))
                {
                    result.Add(PrimitiveType[i]);
                    checkTarget = checkTarget.Replace(PrimitiveType[i], "");
                }
            }

            return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }
        



    }
}
