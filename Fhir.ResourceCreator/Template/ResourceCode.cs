using FhirResourceCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirResourceCreator.Fhir;

namespace FhirResourceCreator.Template
{
    partial class ResourceTemplate
    {
        private readonly string? _className;
        private readonly string? _properties;
        private readonly string? _constructor;
        private readonly string? _setup;
        private readonly string? _namespaceSub;
        private readonly bool _hasChoice = false;


        public ResourceTemplate(string resourceName, OneClassContent source, string saveTo)
        {
            _className = resourceName;
            _namespaceSub = $"{resourceName}Sub";

            _properties = source.PropertyString;
            _constructor = source.ConstructorString;
            _setup = source.SetupString;

            //if (item.IsChoice && !string.IsNullOrEmpty(item.FinalElementName))
            //{
            //    _hasChoice = true;
            //    ChoiceTemplate ct = new(item.FinalElementName, resourceName);
            //    var output = ct.TransformText();
            //    string fullPath = Path.Combine($"{saveTo}\\Choice", $"Choice{item.FinalElementName}.generated.cs");
            //    File.WriteAllText(fullPath, output);
            //}
            //else if (item.IsBackboneElement && !string.IsNullOrEmpty(item.FinalElementName) && !string.IsNullOrEmpty(item.ThisPath))
            //{
            //    BackboneTemplate bt = new(item.FinalElementName, elements, item.ThisPath, _namespaceSub, saveTo, resourceName, $"{saveTo}\\Choice");
            //    var output = bt.TransformText();
            //    string fullPath = Path.Combine(saveTo, $"{item.FinalElementName}.generated.cs");
            //    File.WriteAllText(fullPath, output);

            //}
        }
    }

}
