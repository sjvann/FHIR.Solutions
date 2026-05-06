using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.Interface;

namespace Fhir.TypeFramework.DataTypes;

public class FhirBoolean : BooleanPrimitiveTypeBase, IBooleanValue
{
    public FhirBoolean() { }
    public FhirBoolean(bool v) : base(v) { }
    public FhirBoolean(string? v) : base(v) { }

    public static implicit operator FhirBoolean?(bool? value) => CreateFromBoolean<FhirBoolean>(value);
    public static implicit operator bool?(FhirBoolean? instance) => GetBooleanValue(instance);

    // 向後相容：允許用字串初始化/取值（本質仍以 Value/StringValue 同步）
    public static implicit operator FhirBoolean?(string? value) => value is null ? null : new FhirBoolean(value);
    public static implicit operator string?(FhirBoolean? instance) => instance?.StringValue;

    bool? IValue<bool?>.Value => HasValue ? Value : null;
    public bool HasValue => !IsNull;
}

