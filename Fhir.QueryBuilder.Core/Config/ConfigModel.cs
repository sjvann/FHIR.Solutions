using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fhir.QueryBuilder.Config
{
    public class ConfigModel
    {
        [JsonPropertyName("fhirVersion")]
        public string? FhirVersion { get; set; }
    }
}
