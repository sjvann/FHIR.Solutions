using Fhir.QueryBuilder.QueryBuilders.FluentApi;
using Fhir.QueryBuilder.Services.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Fhir.QueryBuilder.Tests.Integration
{
    public class FhirQueryIntegrationTests : TestBase
    {
        protected override bool EnableTestCaching => true;

        [Fact]
        public void ServiceRegistration_ShouldResolveAllServices()
        {
            // Act & Assert - All services should be resolvable
            var validationService = GetService<IValidationService>();
            validationService.Should().NotBeNull();

            var cacheService = GetService<ICacheService>();
            cacheService.Should().NotBeNull();

            var errorHandlingService = GetService<IErrorHandlingService>();
            errorHandlingService.Should().NotBeNull();

            var progressService = GetService<IProgressService>();
            progressService.Should().NotBeNull();

            var configurationService = GetService<IConfigurationService>();
            configurationService.Should().NotBeNull();

            var queryBuilder = GetService<IFhirQueryBuilder>();
            queryBuilder.Should().NotBeNull();
        }

        [Fact]
        public void FluentApi_EndToEndQuery_ShouldGenerateCorrectUrl()
        {
            // Arrange
            var queryBuilder = GetService<IFhirQueryBuilder>();
            var baseUrl = "https://example.com/fhir";

            // Act
            var result = queryBuilder
                .ForResource("Patient")
                .WhereString("family", "Smith")
                .WhereDate("birthdate", "1990-01-01", SearchPrefix.GreaterEqual)
                .WhereToken("gender", "male")
                .Include("Patient:organization")
                .RevInclude("Observation:patient")
                .Count(50)
                .Summary("true")
                .BuildUrl(baseUrl);

            // Assert
            result.Should().StartWith("https://example.com/fhir/Patient?");
            result.Should().Contain("family=Smith");
            result.Should().Contain("birthdate=ge1990-01-01");
            result.Should().Contain("gender=male");
            result.Should().Contain("_include=Patient:organization");
            result.Should().Contain("_revinclude=Observation:patient");
            result.Should().Contain("_count=50");
            result.Should().Contain("_summary=true");
        }

        [Fact]
        public void ValidationService_WithFluentApi_ShouldValidateGeneratedQueries()
        {
            // Arrange
            var queryBuilder = GetService<IFhirQueryBuilder>();
            var validationService = GetService<IValidationService>();
            var baseUrl = "https://example.com/fhir";

            // Act
            var queryUrl = queryBuilder
                .ForResource("Patient")
                .WhereString("family", "Smith")
                .BuildUrl(baseUrl);

            var validationResult = validationService.ValidateQuery(queryUrl);

            // Assert
            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void ErrorHandling_ShouldCaptureAndReportErrors()
        {
            // Arrange
            var errorHandlingService = GetService<IErrorHandlingService>();
            var errorOccurred = false;
            ErrorInfo? capturedError = null;

            errorHandlingService.ErrorOccurred += (sender, error) =>
            {
                errorOccurred = true;
                capturedError = error;
            };

            // Act
            var testException = new InvalidOperationException("Test error");
            errorHandlingService.HandleError(testException, "TestSource");

            // Assert
            errorOccurred.Should().BeTrue();
            capturedError.Should().NotBeNull();
            capturedError!.Message.Should().Be("Test error");
            capturedError.Source.Should().Be("TestSource");
            capturedError.Exception.Should().Be(testException);
        }

        [Fact]
        public void ProgressService_ShouldTrackOperationProgress()
        {
            // Arrange
            var progressService = GetService<IProgressService>();
            var progressSnapshots = new List<(string Message, int PercentComplete, bool IsCompleted)>();

            progressService.ProgressChanged += (_, progress) =>
            {
                progressSnapshots.Add((progress.Message, progress.PercentComplete, progress.IsCompleted));
            };

            // Act
            var operationId = progressService.StartOperation("TestOperation", "Starting test");
            progressService.UpdateProgress(operationId, "In progress", 50);
            progressService.CompleteOperation(operationId, "Completed");

            // Assert（ProgressInfo 為可變物件，須在事件當下快照）
            progressSnapshots.Should().HaveCount(3);
            progressSnapshots[0].Message.Should().Be("Starting test");
            progressSnapshots[1].PercentComplete.Should().Be(50);
            progressSnapshots[2].IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task CacheService_ShouldStoreAndRetrieveData()
        {
            // Arrange
            var cacheService = GetService<ICacheService>();
            var testData = "Test cache data";
            var cacheKey = "test-key";

            // Act
            await cacheService.SetAsync(cacheKey, testData);
            var retrievedData = await cacheService.GetAsync<string>(cacheKey);

            // Assert
            retrievedData.Should().Be(testData);
        }

        [Fact]
        public async Task CacheService_WithExpiration_ShouldExpireData()
        {
            // Arrange
            var cacheService = GetService<ICacheService>();
            var testData = "Test cache data";
            var cacheKey = "test-key-expiry";

            // Act
            await cacheService.SetAsync(cacheKey, testData, TimeSpan.FromMilliseconds(100));
            await Task.Delay(200); // Wait for expiration
            var retrievedData = await cacheService.GetAsync<string>(cacheKey);

            // Assert
            retrievedData.Should().BeNull();
        }

        [Fact]
        public void QueryBuilder_WithInvalidInput_ShouldProvideValidationErrors()
        {
            // Arrange
            var queryBuilder = GetService<IFhirQueryBuilder>();

            // Act
            var isValid = queryBuilder.IsValid(); // No resource specified
            var errors = queryBuilder.GetValidationErrors();

            // Assert
            isValid.Should().BeFalse();
            errors.Should().NotBeEmpty();
            errors.Should().Contain(e => e.Contains("Resource type must be specified"));
        }

        [Fact]
        public void MultipleQueryBuilders_ShouldBeIndependent()
        {
            // Arrange
            var queryBuilder1 = GetService<IFhirQueryBuilder>();
            var queryBuilder2 = GetService<IFhirQueryBuilder>();

            // Act
            queryBuilder1.ForResource("Patient").WhereString("family", "Smith");
            queryBuilder2.ForResource("Observation").WhereString("code", "12345");

            var query1 = queryBuilder1.BuildQueryString();
            var query2 = queryBuilder2.BuildQueryString();

            // Assert
            query1.Should().Contain("family=Smith");
            query1.Should().NotContain("code=12345");
            
            query2.Should().Contain("code=12345");
            query2.Should().NotContain("family=Smith");
        }
    }
}
