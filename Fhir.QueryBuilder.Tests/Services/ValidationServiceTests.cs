using Fhir.QueryBuilder.Services.Interfaces;
using FluentAssertions;

namespace Fhir.QueryBuilder.Tests.Services
{
    public class ValidationServiceTests : TestBase
    {
        private readonly IValidationService _validationService;

        public ValidationServiceTests()
        {
            _validationService = GetService<IValidationService>();
        }

        [Theory]
        [InlineData("https://example.com/fhir", true)]
        [InlineData("http://localhost:8080/fhir", true)]
        [InlineData("https://server.fire.ly", true)]
        [InlineData("", false)]
        [InlineData("not-a-url", false)]
        [InlineData("ftp://example.com", false)]
        public void ValidateUrl_ShouldReturnExpectedResult(string url, bool expectedValid)
        {
            // Act
            var result = _validationService.ValidateUrl(url);

            // Assert
            result.IsValid.Should().Be(expectedValid);
        }

        [Fact]
        public void ValidateUrl_WithInvalidUrl_ShouldReturnErrors()
        {
            // Arrange
            var invalidUrl = "not-a-valid-url";

            // Act
            var result = _validationService.ValidateUrl(invalidUrl);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("family", "Smith", "string", true)]
        [InlineData("birthdate", "1990-01-01", "date", true)]
        [InlineData("birthdate", "ge1990-01-01", "date", true)]
        [InlineData("birthdate", "invalid-date", "date", false)]
        [InlineData("", "value", "string", false)]
        [InlineData("param", "", "string", false)]
        public void ValidateSearchParameter_ShouldReturnExpectedResult(string paramName, string paramValue, string paramType, bool expectedValid)
        {
            // Act
            var result = _validationService.ValidateSearchParameter(paramName, paramValue, paramType);

            // Assert
            result.IsValid.Should().Be(expectedValid);
        }

        [Fact]
        public void ValidateSearchParameter_WithDateParameter_ShouldValidatePrefixes()
        {
            // Arrange
            var validPrefixes = new[] { "eq1990-01-01", "ge1990-01-01", "lt2000-01-01" };
            var invalidPrefixes = new[] { "invalid1990-01-01", "xyz1990-01-01" };

            // Act & Assert
            foreach (var validPrefix in validPrefixes)
            {
                var result = _validationService.ValidateSearchParameter("birthdate", validPrefix, "date");
                result.IsValid.Should().BeTrue($"'{validPrefix}' should be valid");
            }

            foreach (var invalidPrefix in invalidPrefixes)
            {
                var result = _validationService.ValidateSearchParameter("birthdate", invalidPrefix, "date");
                result.IsValid.Should().BeFalse($"'{invalidPrefix}' should be invalid");
            }
        }

        [Theory]
        [InlineData("Patient", true)]
        [InlineData("Observation", true)]
        [InlineData("patient", false)] // Should warn about case
        [InlineData("", false)]
        public void ValidateResourceType_ShouldReturnExpectedResult(string resourceType, bool expectedValid)
        {
            // Act
            var result = _validationService.ValidateResourceType(resourceType);

            // Assert
            if (expectedValid && resourceType == "patient")
            {
                // Special case: should have warning about case
                result.IsValid.Should().BeTrue();
                result.Warnings.Should().NotBeEmpty();
            }
            else
            {
                result.IsValid.Should().Be(expectedValid);
            }
        }

        [Fact]
        public void ValidateResourceType_WithSupportedResources_ShouldValidateAgainstList()
        {
            // Arrange
            var supportedResources = new[] { "Patient", "Observation", "Practitioner" };

            // Act
            var validResult = _validationService.ValidateResourceType("Patient", supportedResources);
            var invalidResult = _validationService.ValidateResourceType("UnsupportedResource", supportedResources);

            // Assert
            validResult.IsValid.Should().BeTrue();
            invalidResult.IsValid.Should().BeFalse();
            invalidResult.Errors.Should().Contain(e => e.Contains("not supported"));
        }

        [Theory]
        [InlineData("https://example.com/fhir/Patient", true)]
        [InlineData("https://example.com/fhir/Patient?family=Smith", true)]
        [InlineData("", false)]
        [InlineData("not-a-url", false)]
        public void ValidateQuery_ShouldReturnExpectedResult(string query, bool expectedValid)
        {
            // Act
            var result = _validationService.ValidateQuery(query);

            // Assert
            result.IsValid.Should().Be(expectedValid);
        }

        [Fact]
        public void ValidateConnectionSettings_ShouldValidateUrlAndToken()
        {
            // Arrange
            var validUrl = "https://example.com/fhir";
            var validToken = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

            // Act
            var result = _validationService.ValidateConnectionSettings(validUrl, validToken);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateQueryComplexity_WithManyParameters_ShouldWarn()
        {
            // Arrange
            var complexQuery = "https://example.com/fhir/Patient?" + 
                string.Join("&", Enumerable.Range(1, 25).Select(i => $"param{i}=value{i}"));

            // Act
            var result = _validationService.ValidateQueryComplexity(complexQuery);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Warnings.Should().NotBeEmpty();
            result.Warnings.Should().Contain(w => w.Contains("many parameters"));
        }

        [Theory]
        [InlineData("R4", true)]
        [InlineData("R5", true)]
        [InlineData("DSTU2", false)] // Should warn
        [InlineData("", false)]
        public void ValidateFhirVersion_ShouldReturnExpectedResult(string version, bool expectedSupported)
        {
            // Act
            var result = _validationService.ValidateFhirVersion(version);

            // Assert
            if (expectedSupported)
            {
                result.IsValid.Should().BeTrue();
            }
            else if (version == "DSTU2")
            {
                result.IsValid.Should().BeTrue();
                result.Warnings.Should().NotBeEmpty();
            }
            else
            {
                result.IsValid.Should().BeFalse();
            }
        }

        [Fact]
        public void ValidateSearchParameter_Composite_WithTwoComponents_IsValid()
        {
            var result = _validationService.ValidateSearchParameter("combo", "a$b", "composite");

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateSearchParameter_Composite_WithOneComponent_IsInvalid()
        {
            var result = _validationService.ValidateSearchParameter("combo", "only", "composite");

            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void ValidateSearchParameter_Special_ShortValue_HasNoWarnings()
        {
            var result = _validationService.ValidateSearchParameter("specialParam", "expr", "special");

            result.IsValid.Should().BeTrue();
            result.Warnings.Should().BeEmpty();
        }

        [Fact]
        public void ValidateSearchParameter_Special_VeryLong_Warns()
        {
            var longVal = new string('x', 2001);
            var result = _validationService.ValidateSearchParameter("specialParam", longVal, "special");

            result.IsValid.Should().BeTrue();
            result.Warnings.Should().Contain(w => w.Contains("long", StringComparison.OrdinalIgnoreCase));
        }
    }
}
