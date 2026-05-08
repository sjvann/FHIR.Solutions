using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Fhir
{
    public enum KeywordCheckerType
    {
        ForComplex, ForBackbone,None
    }
    public static class KeywordChecker
    {
        const string _ComplexDataTypeNamespace = "Fhir.TypeFramework.DataTypes";
            private static bool _isComplex=false;
        private static bool _isBackbone = false;

        public static string ComplexDataTypeNamespace  => _ComplexDataTypeNamespace; 
        public static KeywordCheckerType CheckKeywords(string? elementName)
        {
            if (string.IsNullOrEmpty(elementName)) { return KeywordCheckerType.None; }

            ImpCheckKeyword(elementName);
            if (_isBackbone)
            {
                return KeywordCheckerType.ForBackbone;
            }
            else if(_isComplex)
            {
                return KeywordCheckerType.ForComplex;
            }
            else
            {
                return KeywordCheckerType.None;
            }

        }
        private static void ImpCheckKeyword(string source)
        {
            _isBackbone = source switch
            {
                "Version" or "Exception" or "Action" or "Element" or "version" or "action"=> true,
                _ => false,
            };
            _isComplex = source switch
            {
                "Range" => true,
                _ => false,
            };
        }
    }
}
