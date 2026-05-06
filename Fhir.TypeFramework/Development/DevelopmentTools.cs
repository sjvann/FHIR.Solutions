using System.ComponentModel.DataAnnotations;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.Extensions;

namespace Fhir.TypeFramework.Development;

public static class DevelopmentTools
{
    public sealed record TypeInfoResult(
        string TypeName,
        string Namespace,
        bool ImplementsIdentifiable,
        bool ImplementsExtensible,
        bool ImplementsValidatable);

    public static TypeInfoResult GetTypeInfo<T>() where T : ITypeFramework
    {
        var t = typeof(T);
        return new TypeInfoResult(
            TypeName: t.Name,
            Namespace: t.Namespace ?? string.Empty,
            ImplementsIdentifiable: typeof(IIdentifiableTypeFramework).IsAssignableFrom(t),
            ImplementsExtensible: typeof(IExtensibleTypeFramework).IsAssignableFrom(t),
            ImplementsValidatable: typeof(IValidatableObject).IsAssignableFrom(t));
    }

    public static (bool IsValid, string[] Errors) ValidateWithDetails(ITypeFramework instance)
    {
        var errors = instance.GetValidationErrors().ToArray();
        return (errors.Length == 0, errors);
    }

    public static string GetUsageExample<T>() where T : ITypeFramework
    {
        var t = typeof(T);
        return $"// {t.Name} usage example (generated)\nvar instance = new {t.Name}();\nvar isValid = instance.IsValid();\n";
    }
}

