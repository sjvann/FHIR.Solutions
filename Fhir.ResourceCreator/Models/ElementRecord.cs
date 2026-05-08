using FhirResourceCreator.Extension;
using FhirResourceCreator.Fhir;
using FhirResourceCreator.Generation;
using System.Text;

namespace FhirResourceCreator.Models
{
    public class ElementRecord
    {

        // Is Flag
        private bool _isBackboneElement = false;
        private bool _isChoice = false;
        private bool _isSkip = false;
        private bool _isPrimitive = false;
        private bool _isReference = false;
        private readonly bool _isMust = false;
        private readonly bool _isMulti = false;
        private readonly KeywordCheckerType _keywordType;

        // Element
        private string? _thisPath;
        private string? _parentPath;

        private string? _jsonElementName;
        private string? _finalElementName;

        private string[]? _elements;

        // DataType
        private string? _originalDataType;
        private string? _finalDataType;


        private int _level;

        private string[]? _referenceDataTypeList = null;
        private string[]? _choiceDataTypeList = null;
        private string? _primitiveClrName;
        

        public ElementRecord(string path, string min, string max, string type)
        {
            _isMulti = max == "*";
            _isMust = Convert.ToInt16(min) > 0;
            SetupForElement(path);
            SetupForType(type);
            
            _keywordType = KeywordChecker.CheckKeywords(_originalDataType);

        }
        #region Public Method
        public string GetProperty(string? addNamespace = null)
        {
            if (!string.IsNullOrEmpty(_finalElementName) && !string.IsNullOrEmpty(_finalDataType))
            {
                if (string.IsNullOrEmpty(addNamespace))
                {
                    return $"public {_finalDataType} {_finalElementName} {{get; set;}}";
                }
                else
                {
                    string newDataTypeName = $"{addNamespace}.{_originalDataType}";
                    if (_isMulti && _originalDataType != null)
                    {
                        newDataTypeName = _finalDataType.Replace(_originalDataType, newDataTypeName);
                    }
                    return $"public {newDataTypeName} {_finalElementName} {{get; set;}}";
                }
            }
            else
            {
                return string.Empty;
            }

        }
        public string GetConstructor(string? addNamespace = null)
        {
            StringBuilder result = new();
            if (_isChoice && _choiceDataTypeList != null && _choiceDataTypeList.Length != 0)
            {
                foreach (var item in _choiceDataTypeList)
                {
                    result.Append("case \"" + _jsonElementName + item.ToUpperFirstLetter() + "\": ");
                }
                result.Append(_finalElementName + " = new(ck, cv); break; ");
            }
            else
            {
                result.Append("case \"" + _jsonElementName + "\": " + _finalElementName + " = ");
                if (_isMulti)
                {
                    if (_isReference)
                    {
                        result.Append("cv?.AsArray().Select(x=>new Reference(x)); break;");
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(addNamespace))
                        {
                            result.Append("cv?.AsArray().Select(x=>new " + _originalDataType + "(x)); break;");
                        }
                        else
                        {
                            string newPropertyName = $"{addNamespace}.{_originalDataType}";
                            result.Append("cv?.AsArray().Select(x=>new " + newPropertyName + "(x)); break;");
                        }
                    }

                }
                else
                {
                    result.Append("new(cv); break;");
                }
            }
            return result.ToString();
        }
        public string GetSetup(string? addNamespace = null)
        {
            StringBuilder result = new();

            if (string.IsNullOrWhiteSpace(addNamespace))
            {
                result.Append("if (" + _finalElementName + " is " + _finalDataType + " " + _jsonElementName + "Value" + ") { result.Add(");
            }
            else
            {
                if (_finalDataType != null && _originalDataType != null)
                {
                    var newDataTypeName = $"{addNamespace}.{_originalDataType}";
                    newDataTypeName = _finalDataType.Replace(_originalDataType, newDataTypeName);

                    result.Append("if (" + _finalElementName + " is " + newDataTypeName + " " + _jsonElementName + "Value" + ") { result.Add(");
                }

            }


            if (_isMulti)
            {
                result.Append("ForArrayType(\"" + _jsonElementName + "\", " + _jsonElementName + "Value)); }");
            }
            else if (_isPrimitive)
            {
                result.Append("ForPrimitiveType(\"" + _jsonElementName + "\", " + _jsonElementName + "Value)); }");
            }
            else if (_isChoice)
            {

                result.Append(_jsonElementName + "Value.GetProperty()); }");
            }
            else
            {
                result.Append("ForComplexType(\"" + _jsonElementName + "\", " + _jsonElementName + "Value)); }");
            }
            return result.ToString();
        }
        public string GetElementByLevel(int level)
        {
            return (_elements != null && _elements.Length != 0) ? _elements[level] : string.Empty;
        }
        #endregion
        public int LastLevel => _level;
        public string[]? Elements => _elements;
        public bool IsPrimitive => _isPrimitive;
        public bool IsMulti => _isMulti;
        public bool IsChoice => _isChoice;
        public bool IsBackboneElement => _isBackboneElement;
        public bool IsMust => _isMust;
        public bool IsSkip => _isSkip;
        public string? ParentPath => _parentPath;
        public string? ThisPath => _thisPath;
        public string[]? ResourcesInReference => _referenceDataTypeList;
        public string? OriginalDataType => _originalDataType;
        public string? OriginalElementName => _jsonElementName;
        public string? FinalElementName => _finalElementName;
        public string? FinalDataType => _finalDataType;
        public KeywordCheckerType KeywordType => _keywordType;

        /// <summary>Primitive CLR type name (e.g. FhirString) when <see cref="IsPrimitive"/>.</summary>
        public string? MappedPrimitiveClr => _primitiveClrName;

        /// <summary>Unqualified CLR name for POCO emission (primitive or complex).</summary>
        public string ClrTypeSimpleName =>
            _isPrimitive ? (_primitiveClrName ?? "object") : (_originalDataType ?? "object");

        public IReadOnlyList<string> ChoiceTypeCodes =>
            _choiceDataTypeList ?? Array.Empty<string>();

        #region Private Method
        private void SetupForElement(string path)
        {

            if (path.Contains("[x]"))
            {
                _isChoice = true;
                _thisPath = path.Replace("[x]", string.Empty);
            }
            else
            {
                _thisPath = path;
            }

            if (!string.IsNullOrEmpty(_thisPath))
            {
                _elements = _thisPath.Split('.');
                if (_elements != null && _elements.Length > 0)
                {
                    _level = _elements.Length - 1;
                    _jsonElementName = _elements.Last();
                    var _originalElementName = _jsonElementName.ToUpperFirstLetter();
                    var pElementName = _elements[..^1].Last();
                    _finalElementName = (pElementName == _jsonElementName) ? $"{_originalElementName}Prop" : _originalElementName;
                    _parentPath = string.Join(".", _elements[..^1]);
                }
            }
            _isSkip = CheckSkipElement(_jsonElementName, _level);
        }
        private void SetupForType(string type)
        {

            if (_isChoice)
            {
                _originalDataType = $"Choice{_jsonElementName?.ToUpperFirstLetter()}";
                _choiceDataTypeList = DataTypes.CheckDataTypesFormString(type);

            }
            else if (type.StartsWith("Reference"))
            {
                _isReference = true;
                _originalDataType = "Reference";
                _referenceDataTypeList = type.GetStringBetweenTwoChars('(', ')').Split('|');

            }
            else if (type == "BackboneElement")
            {
                _isBackboneElement = true;
                _originalDataType = _jsonElementName?.ToUpperFirstLetter();
            }
            else
            {
                string pureDataType;
                if (type.Contains('('))
                {
                    pureDataType = type.RemoveSomeString('(', ')');
                }
                else if (type.Contains('{'))
                {
                    pureDataType = type.RemoveSomeString('{', '}');
                }
                else
                {
                    pureDataType = type;
                }
                    var lowerPrim = pureDataType.ToLowerInvariant();
                    if (PrimitiveTypeMapper.IsPrimitiveCode(lowerPrim))
                    {
                        _isPrimitive = true;
                        _primitiveClrName = PrimitiveTypeMapper.ToClrTypeName(lowerPrim);
                        _originalDataType = _primitiveClrName;
                    }
                    else if (pureDataType.Length > 0 && char.IsUpper(pureDataType[0]))
                    {
                        _originalDataType = pureDataType;
                    }
                    else
                    {
                        _originalDataType = pureDataType.ToUpperFirstLetter();
                    }

            }
            if (_isMulti)
            {
                _finalDataType = $"List<{_originalDataType}>";
            }
            else
            {
                _finalDataType = _originalDataType;
            }
        }
        private bool CheckSkipElement(string? elementName, int level)
        {
            if (elementName == "language" && level == 1) return true;
            return elementName switch
            {
                "id" or "meta" or "implicitRules" or "text" or "contained" or "extension" or "modifierExtension" => true,
                _ => false,
            };
        }

        #endregion




    }
}
