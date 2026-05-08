using Fhir.Resources.R5;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.Terminology.Core;

internal static class CapabilityResources
{
    public static CapabilityStatement.RestComponent.RestResourceComponent CodeSystem() =>
        new()
        {
            Type = new FhirCode("CodeSystem"),
            Interaction = Interactions(["read", "search-type", "create", "update", "delete"]),
            SearchParam = StandardSearchParams(),
            Operation = Ops(["$lookup", "$validate-code", "$subsumes"]),
        };

    public static CapabilityStatement.RestComponent.RestResourceComponent ValueSet() =>
        new()
        {
            Type = new FhirCode("ValueSet"),
            Interaction = Interactions(["read", "search-type", "create", "update", "delete"]),
            SearchParam = StandardSearchParams(),
            Operation = Ops(["$expand", "$validate-code"]),
        };

    public static CapabilityStatement.RestComponent.RestResourceComponent ConceptMap() =>
        new()
        {
            Type = new FhirCode("ConceptMap"),
            Interaction = Interactions(["read", "search-type", "create", "update", "delete"]),
            SearchParam = StandardSearchParams(),
            Operation = Ops(["$translate"]),
        };

    private static List<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceInteractionComponent> Interactions(string[] codes)
    {
        var list = new List<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceInteractionComponent>();
        foreach (var c in codes)
        {
            list.Add(new CapabilityStatement.RestComponent.RestResourceComponent.RestResourceInteractionComponent { Code = new FhirCode(c) });
        }

        return list;
    }

    private static List<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceSearchParamComponent> StandardSearchParams()
    {
        string[] names = ["url", "version", "name", "title", "status"];
        var list = new List<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceSearchParamComponent>();
        foreach (var n in names)
        {
            list.Add(new CapabilityStatement.RestComponent.RestResourceComponent.RestResourceSearchParamComponent
            {
                Name = new FhirString(n),
                Type = new FhirCode("string"),
            });
        }

        return list;
    }

    private static List<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceOperationComponent> Ops(string[] names)
    {
        var list = new List<CapabilityStatement.RestComponent.RestResourceComponent.RestResourceOperationComponent>();
        foreach (var n in names)
        {
            list.Add(new CapabilityStatement.RestComponent.RestResourceComponent.RestResourceOperationComponent { Name = new FhirString(n) });
        }

        return list;
    }
}
