using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Template
{
    partial class ChoiceTemplate
    {
        private readonly string? _namespace;
        private readonly string? _className;
        public ChoiceTemplate(string className, string choiceNamespace) {
            _namespace = choiceNamespace;
            if(className.Contains("Prop"))
            {
                _className = $"Choice{className.Replace("Prop", "")}";
            }
            else
            {
                 _className = $"Choice{className}";
            }
           
        }
    }
}
