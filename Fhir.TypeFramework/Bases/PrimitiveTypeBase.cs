using System.Globalization;

namespace Fhir.TypeFramework.Bases;

public abstract class PrimitiveTypeBase<TValue> : PrimitiveType<TValue>
{
    protected PrimitiveTypeBase() { }

    protected PrimitiveTypeBase(TValue? value)
    {
        Value = value;
    }

    protected PrimitiveTypeBase(string? value)
    {
        StringValue = value;
    }

    protected static CultureInfo InvariantCulture => CultureInfo.InvariantCulture;
}

