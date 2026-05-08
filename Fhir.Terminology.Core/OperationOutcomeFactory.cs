using System.Net;
using Fhir.Resources.R5;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.Terminology.Core;

public static class OperationOutcomeFactory
{
    public static OperationOutcome Issue(
        string severity,
        string code,
        string diagnostics,
        HttpStatusCode suggestedStatus = HttpStatusCode.BadRequest)
        => new()
        {
            Issue =
            [
                new OperationOutcome.IssueComponent
                {
                    Severity = new FhirCode(severity),
                    Code = new FhirCode(code),
                    Diagnostics = new FhirString(diagnostics),
                },
            ],
        };

    public static OperationOutcome Error(string diagnostics, HttpStatusCode suggestedStatus = HttpStatusCode.BadRequest)
        => Issue("error", "processing", diagnostics, suggestedStatus);

    public static OperationOutcome NotFound(string diagnostics)
        => Issue("error", "not-found", diagnostics, HttpStatusCode.NotFound);
}
