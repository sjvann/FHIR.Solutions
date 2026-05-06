using Fhir.TypeFramework.Tests.Application.Contracts;
using Fhir.TypeFramework.Validation;

namespace Fhir.TypeFramework.Tests.Application.BehaviorSuites;

public sealed class ValidationBehaviorSuite : IValidationBehaviorSuite
{
    public void VerifyFhirIdRules()
    {
        Assert.True(ValidationFramework.ValidateFhirId("patient-123"));
        Assert.False(ValidationFramework.ValidateFhirId("bad id"));
    }

    public void VerifyBasicValidationHelpers()
    {
        Assert.True(ValidationFramework.ValidateStringLength("abc", 3));
        Assert.False(ValidationFramework.ValidateStringLength("abcd", 3));

        Assert.True(ValidationFramework.ValidateStringByteSize("abc", 3));
        Assert.True(ValidationFramework.ValidateRegex("abc", "^a"));
        Assert.True(ValidationFramework.ValidatePositiveInteger(1));

        Assert.True(ValidationFramework.ValidateFhirUri("http://example.com"));
        Assert.True(ValidationFramework.ValidateFhirCanonical("http://example.com"));
        Assert.True(ValidationFramework.ValidateFhirOid("1.2.3.4"));
        Assert.True(ValidationFramework.ValidateFhirUuid(Guid.NewGuid().ToString()));
        Assert.True(ValidationFramework.ValidateFhirBase64Binary(Convert.ToBase64String([1, 2, 3])));
    }

    public void VerifyFhirUriAndCodeBranches()
    {
        Assert.True(ValidationFramework.ValidateFhirUri("urn:uuid:" + Guid.NewGuid()));
        Assert.True(ValidationFramework.ValidateFhirUri("relative/path"));
        Assert.True(ValidationFramework.ValidateFhirUri("http://example.com"));

        Assert.True(ValidationFramework.ValidateFhirCode("abc"));
        Assert.False(ValidationFramework.ValidateFhirCode(" abc "));
        Assert.False(ValidationFramework.ValidateFhirCode("a\u0001b"));
    }
}
