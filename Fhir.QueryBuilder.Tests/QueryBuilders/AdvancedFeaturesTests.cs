using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Fhir.QueryBuilder.Tests.QueryBuilders
{
    /// <summary>
    /// 進階功能測試
    /// </summary>
    public class AdvancedFeaturesTests : TestBase
    {
        public AdvancedFeaturesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Chain_ShouldBuildCorrectParameter()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Observation")
                .Chain("patient.name", "John")
                .BuildQueryString();

            // Assert
            Assert.Contains("patient.name=John", query);
            Output.WriteLine($"Chain query: {query}");
        }

        [Fact]
        public void ReverseChain_ShouldBuildCorrectParameter()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Patient")
                .ReverseChain("Observation", "patient", "code=xyz")
                .BuildQueryString();

            // Assert
            Assert.Contains("_has=", query);
            Assert.Contains("Observation", query);
            Assert.Contains("patient", query);
            Assert.Contains("code%3Dxyz", query);
            Output.WriteLine($"Reverse chain query: {query}");
        }

        [Fact]
        public void WhereComposite_ShouldBuildCorrectParameter()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Observation")
                .WhereComposite("component-code-value-quantity", 
                    "http://loinc.org|8480-6", 
                    "120", 
                    "mmHg")
                .BuildQueryString();

            // Assert
            Assert.Contains("component-code-value-quantity=", query);
            Assert.Contains("$", query); // Composite separator
            Output.WriteLine($"Composite query: {query}");
        }

        [Fact]
        public void Filter_ShouldBuildCorrectParameter()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Patient")
                .Filter("name co 'John' and birthDate ge 1990-01-01")
                .BuildQueryString();

            // Assert
            Assert.Contains("_filter=", query);
            Output.WriteLine($"Filter query: {query}");
        }

        [Fact]
        public void Offset_ShouldBuildCorrectParameter()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Patient")
                .Count(50)
                .Offset(100)
                .BuildQueryString();

            // Assert
            Assert.Contains("_count=50", query);
            Assert.Contains("_offset=100", query);
            Output.WriteLine($"Paging query: {query}");
        }

        [Fact]
        public void Total_ShouldBuildCorrectParameter()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Patient")
                .Total("estimate")
                .BuildQueryString();

            // Assert
            Assert.Contains("_total=estimate", query);
            Output.WriteLine($"Total query: {query}");
        }

        [Fact]
        public void Contained_ShouldBuildCorrectParameter()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Patient")
                .Contained("false")
                .BuildQueryString();

            // Assert
            Assert.Contains("_contained=false", query);
            Output.WriteLine($"Contained query: {query}");
        }

        [Fact]
        public void ComplexQuery_ShouldCombineAllFeatures()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource("Observation")
                .WhereString("status", "final")
                .Chain("patient.name", "John")
                .WhereComposite("component-code-value-quantity", 
                    "http://loinc.org|8480-6", 
                    "120")
                .Include("Observation:patient")
                .RevInclude("DiagnosticReport:result")
                .Filter("effectiveDateTime ge 2023-01-01")
                .Count(50)
                .Offset(0)
                .Sort("effectiveDateTime", "-status")
                .Total("accurate")
                .Summary("true")
                .Elements("id", "status", "effectiveDateTime", "valueQuantity")
                .BuildQueryString();

            // Assert
            Assert.Contains("status=final", query);
            Assert.Contains("patient.name=John", query);
            Assert.Contains("component-code-value-quantity=", query);
            Assert.Contains("_include=", query);
            Assert.Contains("_revinclude=", query);
            Assert.Contains("_filter=", query);
            Assert.Contains("_count=50", query);
            Assert.Contains("_offset=0", query);
            Assert.Contains("_sort=", query);
            Assert.Contains("_total=accurate", query);
            Assert.Contains("_summary=true", query);
            Assert.Contains("_elements=", query);
            
            Output.WriteLine($"Complex query: {query}");
        }

        [Fact]
        public void BuildUrl_ShouldCreateCompleteUrl()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();
            var baseUrl = "https://hapi.fhir.org/baseR4";

            // Act
            var url = builder
                .ForResource("Patient")
                .WhereString("name", "John")
                .Count(10)
                .BuildUrl(baseUrl);

            // Assert
            Assert.StartsWith(baseUrl, url);
            Assert.Contains("/Patient", url);
            Assert.Contains("?", url);
            Assert.Contains("name=John", url);
            Assert.Contains("_count=10", url);
            
            Output.WriteLine($"Complete URL: {url}");
        }

        [Theory]
        [InlineData("Patient", "name", "John", "name=John")]
        [InlineData("Observation", "status", "final", "status=final")]
        [InlineData("DiagnosticReport", "subject", "Patient/123", "subject=Patient/123")]
        public void BasicParameters_ShouldBuildCorrectly(string resourceType, string paramName, string value, string expectedContains)
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var query = builder
                .ForResource(resourceType)
                .Where(paramName, value)
                .BuildQueryString();

            // Assert
            Assert.Contains(expectedContains, query);
            Output.WriteLine($"Query for {resourceType}: {query}");
        }

        [Fact]
        public void Validation_ShouldDetectErrors()
        {
            // Arrange
            var builder = ServiceProvider.GetRequiredService<IFhirQueryBuilder>();

            // Act
            var isValid = builder.IsValid(); // 沒有設定資源類型

            // Assert
            Assert.False(isValid);
            
            var errors = builder.GetValidationErrors();
            Assert.NotEmpty(errors);
            
            Output.WriteLine($"Validation errors: {string.Join(", ", errors)}");
        }
    }
}
