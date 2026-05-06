using System.ComponentModel.DataAnnotations;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.Extensions;

public static class TypeFrameworkExtensions
{
    // Extension 相關
    public static IExtension CreateExtension(this IExtensibleTypeFramework extensible, string url, object? value)
    {
        if (extensible is not Element element)
            throw new InvalidOperationException("Extension 只能套用在 Element 及其衍生型別上。");

        element.Extension ??= new List<IExtension>();

        var ext = new Extension
        {
            Url = new FhirString(url),
            Value = value
        };

        element.Extension.Add(ext);
        return ext;
    }

    public static bool HasExtension(this IExtensibleTypeFramework extensible, string url)
        => extensible is Element { Extension: { Count: > 0 } ext } && ext.Any(e => string.Equals(e.Url, url, StringComparison.Ordinal));

    public static bool RemoveExtension(this IExtensibleTypeFramework extensible, string url)
    {
        if (extensible is not Element { Extension: { Count: > 0 } ext }) return false;
        return ext.RemoveAll(e => string.Equals(e.Url, url, StringComparison.Ordinal)) > 0;
    }

    public static T? GetExtensionValue<T>(this IExtensibleTypeFramework extensible, string url) where T : class
    {
        if (extensible is not Element { Extension: { Count: > 0 } ext }) return null;
        var match = ext.OfType<Extension>().FirstOrDefault(e => string.Equals(e.Url?.StringValue, url, StringComparison.Ordinal));
        return match?.Value as T;
    }

    // 驗證相關
    public static bool IsValid(this ITypeFramework instance)
        => !instance.GetValidationErrors().Any();

    public static IEnumerable<string> GetValidationErrors(this ITypeFramework instance)
    {
        if (instance is IValidatableObject validatable)
        {
            var ctx = new ValidationContext(instance);
            foreach (var r in validatable.Validate(ctx))
            {
                if (!string.IsNullOrWhiteSpace(r.ErrorMessage))
                    yield return r.ErrorMessage!;
            }
        }
    }

    public static void ValidateAndThrow(this ITypeFramework instance)
    {
        var errors = instance.GetValidationErrors().ToArray();
        if (errors.Length == 0) return;
        throw new ValidationException(string.Join(Environment.NewLine, errors));
    }
}

