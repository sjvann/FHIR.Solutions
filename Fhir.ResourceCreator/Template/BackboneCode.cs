using FhirResourceCreator.Fhir;
using FhirResourceCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Template
{
    partial class BackboneTemplate
    {
        private readonly string? _properties;
        private readonly string? _constructor;
        private readonly string? _setup;
        private readonly string? _className;
        private readonly string? _namespaceSub;
        private readonly bool _needUsing = false;
        private readonly string? _resourceName;
        private readonly bool _hasChoice = false;
        private bool _needResourceBased;
 


        public BackboneTemplate(string className, IEnumerable<ElementRecord> source,string parentPath, string namespaceSub, string saveTo, string resourceName, string choiceSaveTo)
        {
            _namespaceSub = namespaceSub;
            _className = className;
            _resourceName = resourceName;
            StringBuilder sb1 = new();
            StringBuilder sb2 = new();
            StringBuilder sb3 = new();
            _needResourceBased = className == "Entry" && namespaceSub.Contains("Bundle");
          
            var targets = source.Where(x => !x.IsSkip && x.ParentPath == parentPath);
            if (targets != null && targets.Any())
            {
                foreach (ElementRecord item in targets)
                {
                    var property = string.Empty;
                    var constructor = string.Empty;
                    var setup = string.Empty;
                    Fhir.KeywordCheckerType keywordType = KeywordChecker.CheckKeywords(item.OriginalDataType);
                    switch (keywordType)
                    {
                        case KeywordCheckerType.ForBackbone:
                            property = item.GetProperty($"{_namespaceSub}.{_className}Sub");
                            constructor = item.GetConstructor($"{_namespaceSub}.{_className}Sub");
                            setup = item.GetSetup($"{_namespaceSub}.{_className}Sub");
                            break;
                        case KeywordCheckerType.ForComplex:
                            property = item.GetProperty("DataTypeService.Complex");
                            constructor = item.GetConstructor("DataTypeService.Complex");
                            setup = item.GetSetup("DataTypeService.Complex");
                            break;
                        default:
                            property = item.GetProperty();
                            constructor = item.GetConstructor();
                            setup = item.GetSetup();
                            break;
                    }
                    sb1.AppendLine(property);
                    sb2.AppendLine(constructor);
                    sb3.AppendLine(setup);

                    if (item.IsChoice && !string.IsNullOrEmpty(item.FinalElementName))
                    {
                        _hasChoice = true;
                        ChoiceTemplate ct = new(item.FinalElementName, resourceName);
                        var output = ct.TransformText();
                        string fullPath = Path.Combine(choiceSaveTo, $"Choice{item.FinalElementName}.generated.cs");
                        File.WriteAllText(fullPath, output);
                    }
                    else if (item.IsBackboneElement && !string.IsNullOrEmpty(item.FinalElementName) && !string.IsNullOrEmpty(item.ThisPath))
                    {
                        _needUsing = true;
                        string newFolder = $"{className}Sub";
                        var newSaveTo = $"{saveTo}\\{newFolder}";
                        MakeFolder(newSaveTo);
                        BackboneTemplate bt = new(item.FinalElementName, source, item.ThisPath, $"{_namespaceSub}.{newFolder}", newSaveTo, resourceName, choiceSaveTo);
                        var output = bt.TransformText();
                        string fullPath = Path.Combine(newSaveTo, $"{item.FinalElementName}.generated.cs");
                        File.WriteAllText(fullPath, output);
                    }
                }
            }
            _properties = sb1.ToString();
            _constructor = sb2.ToString();
            _setup = sb3.ToString();
        }

        private void MakeFolder(string v)
        {
            System.IO.Directory.CreateDirectory(v);
        }

    }
}
