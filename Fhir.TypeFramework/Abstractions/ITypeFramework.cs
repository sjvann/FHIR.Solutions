#nullable enable

using System.ComponentModel.DataAnnotations;

namespace Fhir.TypeFramework.Abstractions;

public interface ITypeFramework
{
    ITypeFramework DeepCopy();
    bool IsExactly(ITypeFramework? other);
}

public interface IIdentifiableTypeFramework : ITypeFramework
{
}

public interface IExtensibleTypeFramework : ITypeFramework
{
}

public interface IExtension : ITypeFramework, IValidatableObject
{
    string? Url { get; }
}
