using System.ComponentModel.DataAnnotations;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;

namespace Fhir.TypeFramework.Tests.Support.TestDoubles;

internal sealed class TestElement : Element
{
    public override Base DeepCopy() => (TestElement)MemberwiseClone();

    public override bool IsExactly(Base other) => other is TestElement;
}

internal sealed class ConcreteElement : Element;

internal sealed class ConcreteDataType : Fhir.TypeFramework.Bases.DataType;

internal sealed class TestBackboneElement : BackboneElement
{
    public override Base DeepCopy() => (TestBackboneElement)MemberwiseClone();

    public override bool IsExactly(Base other) => other is TestBackboneElement;
}

internal sealed class ConcreteBackboneElement : BackboneElement;

internal sealed class TestBackboneType : BackboneType;

internal sealed class TestBase : Base
{
    public override Base DeepCopy() => new TestBase();
    public override bool IsExactly(Base other) => other is TestBase;
}

internal sealed class TestComplexType : ComplexTypeBase
{
    public FhirString? Child { get; set; }

    protected override void DeepCopyInternal(ComplexTypeBase copy)
    {
        var typed = (TestComplexType)copy;
        typed.Child = Child?.DeepCopy() as FhirString;
    }

    protected override bool IsExactlyInternal(ComplexTypeBase other)
    {
        var typed = (TestComplexType)other;
        return (Child == null && typed.Child == null) || (Child != null && typed.Child != null && Child.IsExactly(typed.Child));
    }

    protected override IEnumerable<ValidationResult> ValidateInternal(ValidationContext validationContext)
    {
        if (Child is null)
            yield return new ValidationResult("Child required", [nameof(Child)]);
    }

    public static bool PublicAreListsEqual(IList<TestBase>? a, IList<TestBase>? b) => AreListsEqual(a, b);

    public static IEnumerable<ValidationResult> PublicValidateItem(TestBase? item) =>
        ValidateItem(item, new ValidationContext(item ?? new object()));

    public static IEnumerable<ValidationResult> PublicValidateList(IList<TestBase>? list) =>
        ValidateList(list, new ValidationContext(list ?? new object()));

    public static IList<FhirString>? PublicDeepCopyList(IList<FhirString>? source) => DeepCopyList(source);
}

internal sealed class TestResource : Resource;

internal sealed class TestBoolPrimitive : BooleanPrimitiveTypeBase
{
    public TestBoolPrimitive() { }

    public TestBoolPrimitive(string? value) : base(value) { }

    public static TestBoolPrimitive? PublicCreateFromBoolean(bool? value) => CreateFromBoolean<TestBoolPrimitive>(value);

    public static bool? PublicGetBooleanValue(BooleanPrimitiveTypeBase? instance) => GetBooleanValue(instance);
}

internal sealed class TestStringPrimitive : StringPrimitiveTypeBase
{
    public TestStringPrimitive() { }

    public TestStringPrimitive(string? value) : base(value) { }

    protected override bool ValidateStringValue(string value) => true;

    public static TestStringPrimitive? PublicCreateFromString(string? value) => CreateFromString<TestStringPrimitive>(value);

    public static string? PublicGetStringValue(StringPrimitiveTypeBase? instance) => GetStringValue(instance);
}

internal sealed class TestIntPrimitive : NumericPrimitiveTypeBase<int>
{
    public TestIntPrimitive() { }

    public TestIntPrimitive(int value) : base(value) { }

    public TestIntPrimitive(string? value) : base(value) { }

    protected override bool ValidateNumericValue(int value) => value >= 0;

    public static TestIntPrimitive? PublicCreateFromNumber(int? value) => CreateFromNumber<TestIntPrimitive>(value);

    public static int? PublicGetNumericValue(NumericPrimitiveTypeBase<int>? instance) => GetNumericValue(instance);
}

internal sealed class TestDateTimePrimitive : DateTimePrimitiveTypeBase<DateTime>
{
    public TestDateTimePrimitive() { }

    public TestDateTimePrimitive(DateTime value) : base(value) { }

    public TestDateTimePrimitive(string? value) : base(value) { }

    protected override bool ValidateDateTimeValue(DateTime value) => value >= DateTime.UnixEpoch;

    public static TestDateTimePrimitive? PublicCreateFromDateTime(DateTime? value) =>
        CreateFromDateTime<TestDateTimePrimitive>(value);

    public static DateTime? PublicGetDateTimeValue(DateTimePrimitiveTypeBase<DateTime>? instance) =>
        GetDateTimeValue(instance);
}
