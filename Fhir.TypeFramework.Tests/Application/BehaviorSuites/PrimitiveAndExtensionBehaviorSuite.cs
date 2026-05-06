using System.ComponentModel.DataAnnotations;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Fhir.TypeFramework.Extensions;
using Fhir.TypeFramework.Tests.Application.Contracts;
using Fhir.TypeFramework.Tests.Support.TestDoubles;

namespace Fhir.TypeFramework.Tests.Application.BehaviorSuites;

public sealed class PrimitiveAndExtensionBehaviorSuite : IPrimitiveAndExtensionBehaviorSuite
{
    public void VerifyIntegerValidateSkipsWhenStringValueNull()
    {
        var i = new FhirInteger();
        var results = i.Validate(new ValidationContext(i)).ToList();
        Assert.Empty(results);
    }

    public void VerifyExtensionValidationErrorsForInvalidId()
    {
        var id = new FhirId("bad id");
        Assert.False(id.IsValid());
        Assert.NotEmpty(id.GetValidationErrors());
    }

    public void VerifyExtensionRequiresUrl()
    {
        var ext = new Extension();
        var results = ext.Validate(new ValidationContext(ext)).ToList();
        Assert.NotEmpty(results);
    }

    public void VerifyPrimitiveIValueSemantics()
    {
        var i = new FhirInteger();
        Assert.False(i.HasValue);
        Assert.Null(((Fhir.TypeFramework.Interface.IValue<int?>)i).Value);

        i.StringValue = "123";
        Assert.True(i.HasValue);
        Assert.Equal(123, ((Fhir.TypeFramework.Interface.IValue<int?>)i).Value);
    }

    public void VerifyExtensionCrudOnElement()
    {
        var e = new TestElement();
        Assert.False(e.HasExtension("u"));

        var created = e.CreateExtension("u", "v");
        Assert.NotNull(created);
        Assert.True(e.HasExtension("u"));
        Assert.Equal("v", e.GetExtensionValue<string>("u"));
        Assert.True(e.RemoveExtension("u"));
        Assert.False(e.HasExtension("u"));
    }

    public void VerifyValidateAndThrowForInvalidId()
    {
        var id = new FhirId("bad id");
        Assert.Throws<ValidationException>(() => id.ValidateAndThrow());
    }
}
