using System.ComponentModel.DataAnnotations;
using Fhir.TypeFramework.Abstractions;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Fhir.TypeFramework.Validation;
using Fhir.TypeFramework.Tests.Application.Contracts;
using Fhir.TypeFramework.Tests.Support.TestDoubles;

namespace Fhir.TypeFramework.Tests.Application.BehaviorSuites;

public sealed class BackboneAndHierarchyBehaviorSuite : IBackboneAndHierarchyBehaviorSuite
{
    public void VerifyBackboneElementAndBackboneTypeModifierExtensions()
    {
        var be = new TestBackboneElement();
        Assert.False(be.HasModifierExtensions);
        Assert.Null(be.GetModifierExtension("u"));

        be.AddModifierExtension("u", "v");
        Assert.True(be.HasModifierExtensions);
        Assert.NotNull(be.GetModifierExtension("u"));
        Assert.True(be.RemoveModifierExtension("u"));
        Assert.False(be.HasModifierExtensions);

        var bt = new TestBackboneType();
        Assert.False(bt.HasModifierExtensions);
        bt.AddModifierExtension("u", "v");
        Assert.True(bt.HasModifierExtensions);
        Assert.True(bt.RemoveModifierExtension("u"));

        var copy = bt.DeepCopy();
        Assert.True(bt.IsExactly(copy));
        _ = bt.Validate(new ValidationContext(bt)).ToList();
    }

    public void VerifyBackboneElementIsExactlyAndValidate()
    {
        var a = new ConcreteBackboneElement();
        var b = new ConcreteBackboneElement();

        a.AddModifierExtension("http://x", "v");
        b.AddModifierExtension("http://x", "v");

        Assert.True(a.IsExactly(b));

        a.ModifierExtension!.Add(new Extension { Url = null, Value = "x" });
        var results = a.Validate(new ValidationContext(a)).ToList();
        Assert.NotEmpty(results);
    }

    public void VerifyBaseExplicitITypeFrameworkMethods()
    {
        ITypeFramework x = new TestBase();
        var copy = x.DeepCopy();
        Assert.True(x.IsExactly(copy));
    }

    public void VerifyComplexTypeBaseDeepCopyList()
    {
        var list = new List<FhirString> { new("a"), new("b") };
        var copy = TestComplexType.PublicDeepCopyList(list);
        Assert.NotNull(copy);
        Assert.Equal(list.Count, copy!.Count);
        Assert.NotSame(list, copy);
    }

    public void VerifyBooleanPrimitiveParsingAndHelpers()
    {
        var t = new TestBoolPrimitive("1");
        Assert.True(t.Value);
        Assert.Equal("1", t.StringValue);

        t.Value = true;
        Assert.Equal("true", t.StringValue);

        Assert.Null(t.ParseValue("not-bool"));
        Assert.Equal(false, t.ParseValue("0"));

        var created = TestBoolPrimitive.PublicCreateFromBoolean(true);
        Assert.NotNull(created);
        Assert.True(created!.Value);
        Assert.True(TestBoolPrimitive.PublicGetBooleanValue(created));
        Assert.Null(TestBoolPrimitive.PublicGetBooleanValue(null));
    }

    public void VerifyElementPrimitiveNumericDateTimeBaseClasses()
    {
        var e1 = new ConcreteElement
        {
            Id = new FhirString("id1"),
            Extension = new List<IExtension> { new Extension { Url = "http://ok", Value = "v" } }
        };
        var e2 = (ConcreteElement)e1.DeepCopy();
        Assert.True(e1.IsExactly(e2));
        Assert.Empty(e1.Validate(new ValidationContext(e1)));

        e2.Extension!.Add(new Extension { Url = "http://ok2", Value = "v2" });
        Assert.False(e1.IsExactly(e2));

        var p = new TestStringPrimitive("abc")
        {
            Id = new FhirString("pid"),
            Extension = new List<IExtension> { new Extension { Url = "http://ext", Value = "x" } }
        };
        var jsonValue = p.ToJsonValue();
        p.FromJsonValue(jsonValue);

        var full = p.ToFullJsonObject();
        Assert.NotNull(full);
        var p2 = new TestStringPrimitive();
        p2.FromFullJsonObject(full);
        _ = p2.ToString();
        _ = p2.Validate(new ValidationContext(p2)).ToList();

        var n = new TestIntPrimitive("123");
        Assert.Equal(123, n.Value);
        Assert.NotNull(n.ParseValue("456"));
        Assert.Null(n.ParseValue("not-int"));
        Assert.True(n.IsValidValue(1));
        Assert.False(n.IsValidValue(-1));
        Assert.NotNull(TestIntPrimitive.PublicCreateFromNumber(7));
        Assert.Equal(7, TestIntPrimitive.PublicGetNumericValue(new TestIntPrimitive(7)));

        var dt = new TestDateTimePrimitive(DateTime.UnixEpoch);
        Assert.NotNull(dt.ParseValue("2020-01-01"));
        Assert.Null(dt.ParseValue("not-dt"));
        Assert.True(dt.IsValidValue(DateTime.UnixEpoch));
        Assert.NotNull(TestDateTimePrimitive.PublicCreateFromDateTime(DateTime.UnixEpoch));
        Assert.Equal(DateTime.UnixEpoch, TestDateTimePrimitive.PublicGetDateTimeValue(dt));
    }

    public void VerifyStringPrimitiveAndDataTypeBranches()
    {
        var created = TestStringPrimitive.PublicCreateFromString("x");
        Assert.NotNull(created);
        Assert.Equal("x", TestStringPrimitive.PublicGetStringValue(created));

        var big = new string('a', (1024 * 1024) + 1);
        Assert.False(created!.IsValidValue(big));

        var dt1 = new ConcreteDataType { Id = new FhirString("a") };
        var dt2 = new ConcreteDataType { Id = new FhirString("b") };
        Assert.False(dt1.IsExactly(dt2));
        _ = dt1.Validate(new ValidationContext(dt1)).ToList();
    }

    public void VerifyResourceIsExactlyAndValidateBranches()
    {
        var r1 = new TestResource
        {
            Id = new FhirId("rid"),
            ImplicitRules = new FhirUri("http://rules"),
            Language = new FhirCode("en")
        };
        var r2 = (TestResource)r1.DeepCopy();
        Assert.True(r1.IsExactly(r2));

        r2.Language = new FhirCode("fr");
        Assert.False(r1.IsExactly(r2));

        _ = r1.Validate(new ValidationContext(r1)).ToList();
    }

    public void VerifyCoverBaseComplexTypeResourceAndMessages()
    {
        var b = new TestBase();
        _ = b.Validate(new ValidationContext(b)).ToList();

        var ct = new TestComplexType { Child = new FhirString("x") };
        var ctCopy = ct.DeepCopy();
        Assert.True(ct.IsExactly(ctCopy));
        _ = ct.Validate(new ValidationContext(ct)).ToList();

        Assert.True(TestComplexType.PublicAreListsEqual(
            new List<TestBase> { new() },
            new List<TestBase> { new() }));

        _ = TestComplexType.PublicValidateItem(new TestBase()).ToList();
        _ = TestComplexType.PublicValidateList(new List<TestBase> { new() }).ToList();

        var r = new TestResource { Id = new FhirId("id") };
        var rCopy = r.DeepCopy();
        Assert.True(r.IsExactly(rCopy));
        _ = r.Validate(new ValidationContext(r)).ToList();

        _ = ValidationFramework.ValidateExtension(null, new ValidationContext(new object())).ToList();

        _ = ValidationMessages.Formatters.StringTooLong(1);
        _ = ValidationMessages.Formatters.StringTooLargeBytes(1);
    }

    public void VerifyCoverElementBackboneTypeResourceValidateBranches()
    {
        var el = new TestElement
        {
            Id = new FhirString("x"),
            Extension = new List<IExtension>
            {
                new Extension { Url = "http://example.com/ext" }
            }
        };
        _ = el.Validate(new ValidationContext(el)).ToList();

        var bt = new TestBackboneType
        {
            ModifierExtension = new List<IExtension>
            {
                new Extension { Url = null },
                new Extension { Url = "http://ok" }
            }
        };
        _ = bt.Validate(new ValidationContext(bt)).ToList();

        var r = new TestResource
        {
            Id = new FhirId("rid"),
            ImplicitRules = new FhirUri("http://rules"),
            Language = new FhirCode("en")
        };
        _ = r.Validate(new ValidationContext(r)).ToList();

        Assert.Equal(nameof(TestResource), r.TypeName);
    }
}
