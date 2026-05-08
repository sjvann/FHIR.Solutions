using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using FluentAssertions;

namespace Fhir.QueryBuilder.Tests;

public class FluentApiTests : TestBase
{
    [Fact]
    public void QueryBuilder_ForResource_ShouldSetResourceType()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();

        // Act
        var result = builder.ForResource("Patient");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void QueryBuilder_BuildUrl_WithoutResource_ShouldThrowException()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();

        // Act & Assert
        var action = () => builder.BuildUrl("https://example.com/fhir");
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Resource type must be specified");
    }

    [Fact]
    public void QueryBuilder_BuildUrl_WithResource_ShouldGenerateCorrectUrl()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();
        var baseUrl = "https://example.com/fhir";

        // Act
        var result = builder
            .ForResource("Patient")
            .BuildUrl(baseUrl);

        // Assert
        result.Should().Be("https://example.com/fhir/Patient");
    }

    [Fact]
    public void QueryBuilder_WithStringParameter_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();
        var baseUrl = "https://example.com/fhir";

        // Act
        var result = builder
            .ForResource("Patient")
            .WhereString("family", "Smith")
            .BuildUrl(baseUrl);

        // Assert
        result.Should().Be("https://example.com/fhir/Patient?family=Smith");
    }

    [Fact]
    public void QueryBuilder_WithMultipleParameters_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();
        var baseUrl = "https://example.com/fhir";

        // Act
        var result = builder
            .ForResource("Patient")
            .WhereString("family", "Smith")
            .WhereString("given", "John")
            .BuildUrl(baseUrl);

        // Assert
        result.Should().Contain("family=Smith");
        result.Should().Contain("given=John");
        result.Should().Contain("&");
    }

    [Fact]
    public void QueryBuilder_WithInclude_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();
        var baseUrl = "https://example.com/fhir";

        // Act
        var result = builder
            .ForResource("Patient")
            .Include("Patient:organization")
            .BuildUrl(baseUrl);

        // Assert
        result.Should().Contain("_include=Patient:organization");
    }

    [Fact]
    public void QueryBuilder_WithDateParameter_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();
        var baseUrl = "https://example.com/fhir";
        var birthDate = new DateTime(1990, 1, 1);

        // Act
        var result = builder
            .ForResource("Patient")
            .WhereDate("birthdate", birthDate, SearchPrefix.GreaterEqual)
            .BuildUrl(baseUrl);

        // Assert
        result.Should().Contain("birthdate=ge1990-01-01");
    }

    [Fact]
    public void QueryBuilder_WithTokenParameter_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();
        var baseUrl = "https://example.com/fhir";

        // Act
        var result = builder
            .ForResource("Patient")
            .WhereToken("gender", "male", "http://hl7.org/fhir/administrative-gender")
            .BuildUrl(baseUrl);

        // Assert
        result.Should().Contain("gender=http://hl7.org/fhir/administrative-gender|male");
    }

    [Fact]
    public void QueryBuilder_WithCount_ShouldGenerateCorrectQuery()
    {
        // Arrange
        var builder = GetService<IFhirQueryBuilder>();
        var baseUrl = "https://example.com/fhir";

        // Act
        var result = builder
            .ForResource("Patient")
            .Count(50)
            .BuildUrl(baseUrl);

        // Assert
        result.Should().Contain("_count=50");
    }
}
