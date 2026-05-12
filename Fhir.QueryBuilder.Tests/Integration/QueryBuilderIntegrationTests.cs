using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using FluentAssertions;
using Fhir.QueryBuilder.Extensions;
using Fhir.QueryBuilder.Services.Interfaces;
using Fhir.QueryBuilder.QueryCore;
using Fhir.VersionManager;

namespace Fhir.QueryBuilder.Tests.Integration
{
    /// <summary>
    /// 查詢建構器整合測試
    /// </summary>
    public class QueryBuilderIntegrationTests : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IVersionAwareQueryBuilder _queryBuilder;
        private readonly IServerCompatibilityService _compatibilityService;

        public QueryBuilderIntegrationTests()
        {
            var services = new ServiceCollection();
            
            // 註冊服務
            services.AddFhirQueryBuilderR5(options =>
            {
                options.EnableValidation = true;
                options.EnableCaching = true;
                options.EnableCompatibilityCheck = false; // 停用以避免實際網路呼叫
                options.EnablePerformanceMonitoring = true;
            });

            // 註冊日誌
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

            _serviceProvider = services.BuildServiceProvider();
            _queryBuilder = _serviceProvider.GetRequiredService<IVersionAwareQueryBuilder>();
            _compatibilityService = _serviceProvider.GetRequiredService<IServerCompatibilityService>();
        }

        [Fact]
        public void ServiceRegistration_ShouldRegisterAllRequiredServices()
        {
            // Assert
            _serviceProvider.GetService<IVersionAwareQueryBuilder>().Should().NotBeNull();
            _serviceProvider.GetService<IServerCompatibilityService>().Should().NotBeNull();
            _serviceProvider.GetService<ICacheService>().Should().NotBeNull();
            _serviceProvider.GetService<IPerformanceMonitoringService>().Should().NotBeNull();
        }

        [Fact]
        public async Task QueryBuilder_ShouldBuildBasicQuery()
        {
            // Arrange
            var serverUrl = "https://hapi.fhir.org/baseR5";

            // Act
            var result = await _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForServer(serverUrl)
                .ForResource("Patient")
                .AddParameter("name", "John")
                .AddParameter("gender", "male")
                .WithPaging(20, 0)
                .WithCompatibilityCheck(false) // 停用相容性檢查
                .BuildAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Query.Should().Contain("name=John");
            result.Query.Should().Contain("gender=male");
            result.Query.Should().Contain("_count=20");
            result.FullUrl.Should().StartWith(serverUrl);
            result.FullUrl.Should().Contain("/Patient?");
        }

        [Fact]
        public async Task QueryBuilder_ShouldValidateParameters()
        {
            // Act
            var validationResult = await _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForResource("Patient")
                .AddParameter("name", "John")
                .ValidateAsync();

            // Assert
            validationResult.Should().NotBeNull();
            // 應該有錯誤，因為沒有設定伺服器 URL
            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().Contain(e => e.Message.Contains("Server URL"));
        }

        [Fact]
        public async Task QueryBuilder_ShouldHandleComplexQuery()
        {
            // Arrange
            var serverUrl = "https://hapi.fhir.org/baseR5";

            // Act
            var result = await _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForServer(serverUrl)
                .ForResource("Observation")
                .AddParameter("subject", "Patient/123")
                .AddParameter("code", "http://loinc.org|8480-6")
                .AddParameter("date", "ge2023-01-01")
                .OrderBy("date", false) // 降序
                .Include("Observation:subject")
                .WithPaging(50, 0)
                .WithCompatibilityCheck(false)
                .BuildAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Query.Should().Contain("subject=Patient%2F123");
            result.Query.Should().Contain("code=http%3A%2F%2Floinc.org%7C8480-6");
            result.Query.Should().Contain("date=ge2023-01-01");
            result.Query.Should().Contain("_sort=-date");
            result.Query.Should().Contain("_include=Observation%3Asubject");
            result.Query.Should().Contain("_count=50");
        }

        [Fact]
        public async Task QueryBuilder_ShouldProvideAlternatives()
        {
            // Arrange
            _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForResource("Patient")
                .AddParameter("nonexistent-param", "value");

            // Act
            var alternatives = await _queryBuilder.GetAlternativesAsync();

            // Assert
            alternatives.Should().NotBeNull();
            // 可能會有替代建議，但具體內容取決於實作
        }

        [Fact]
        public async Task QueryBuilder_ShouldResetCorrectly()
        {
            // Arrange
            _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForResource("Patient")
                .AddParameter("name", "John");

            // Act
            var resetBuilder = _queryBuilder.Reset();

            // 嘗試建構查詢應該失敗，因為沒有設定必要參數
            var result = await resetBuilder.BuildAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().ContainEquivalentOf("validation failed");
        }

        [Fact]
        public async Task QueryBuilder_ShouldHandleMultipleParameters()
        {
            // Arrange
            var serverUrl = "https://hapi.fhir.org/baseR5";

            // Act
            var result = await _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForServer(serverUrl)
                .ForResource("Patient")
                .AddParameter("name", "John")
                .AddParameter("name", "Jane") // 多個相同參數
                .AddParameter("birthdate", "ge1990-01-01")
                .AddParameter("birthdate", "le2000-12-31")
                .WithCompatibilityCheck(false)
                .BuildAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Query.Should().Contain("name=John");
            result.Query.Should().Contain("name=Jane");
            result.Query.Should().Contain("birthdate=ge1990-01-01");
            result.Query.Should().Contain("birthdate=le2000-12-31");
        }

        [Fact]
        public async Task QueryBuilder_ShouldHandleModifiers()
        {
            // Arrange
            var serverUrl = "https://hapi.fhir.org/baseR5";

            // Act
            var result = await _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForServer(serverUrl)
                .ForResource("Patient")
                .AddParameter("name", "exact", "John")
                .AddParameter("birthdate", "missing", "false")
                .WithCompatibilityCheck(false)
                .BuildAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Query.Should().Contain("name:exact=John");
            result.Query.Should().Contain("birthdate:missing=false");
        }

        [Fact]
        public async Task QueryBuilder_ShouldCalculateComplexity()
        {
            // Arrange
            var serverUrl = "https://hapi.fhir.org/baseR5";

            // Act
            var result = await _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForServer(serverUrl)
                .ForResource("Patient")
                .AddParameter("name", "John")
                .AddParameter("gender", "male")
                .AddParameter("birthdate", "ge1990-01-01")
                .Include("Patient:organization")
                .Include("Patient:general-practitioner")
                .RevInclude("Observation:subject")
                .OrderBy("name")
                .OrderBy("birthdate")
                .WithCompatibilityCheck(false)
                .BuildAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Metadata.EstimatedComplexity.Should().BeGreaterThan(1);
            result.Metadata.ParameterCount.Should().Be(3);
        }

        [Fact]
        public void QueryBuilder_ShouldSupportFluentInterface()
        {
            // Act & Assert - 這個測試主要驗證 Fluent API 的可用性
            var builder = _queryBuilder
                .ForVersion(FhirVersion.R5)
                .ForServer("https://example.com")
                .ForResource("Patient")
                .AddParameter("name", "John")
                .WithPaging(10)
                .OrderBy("name")
                .Include("Patient:organization")
                .WithCompatibilityCheck(false);

            // 驗證建構器不為 null（Fluent API 正常工作）
            builder.Should().NotBeNull();
            builder.Should().BeSameAs(_queryBuilder);
        }

        [Fact]
        public async Task PerformanceMonitoring_ShouldTrackOperations()
        {
            // Arrange
            var performanceService = _serviceProvider.GetService<IPerformanceMonitoringService>();
            if (performanceService == null)
            {
                // 如果效能監控未啟用，跳過測試
                return;
            }

            // Act
            var operationId = performanceService.StartOperation("test-query-build");
            
            await Task.Delay(10); // 模擬一些工作
            
            performanceService.EndOperation(operationId, true);

            // 取得統計
            var stats = await performanceService.GetStatisticsAsync("test-query-build");

            // Assert
            stats.Should().NotBeNull();
            stats.TotalExecutions.Should().BeGreaterThan(0);
            stats.SuccessfulExecutions.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CacheService_ShouldCacheAndRetrieve()
        {
            // Arrange
            var cacheService = _serviceProvider.GetRequiredService<ICacheService>();
            var testData = new { Name = "Test", Value = 123 };

            // Act
            await cacheService.SetAsync("test-key", testData, TimeSpan.FromMinutes(5));
            var retrieved = await cacheService.GetAsync<object>("test-key");

            // Assert
            retrieved.Should().NotBeNull();
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }
    }
}
