using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirResourceCreator.Models
{
    public record OneClassContent
    {
        public string? PropertyString { get; set; }
        public string? ConstructorString { get; set; }
        public string? SetupString { get; set; }

    }
}
