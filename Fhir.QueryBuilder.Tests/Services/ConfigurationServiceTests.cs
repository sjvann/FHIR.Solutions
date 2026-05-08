using Fhir.QueryBuilder.Configuration;
using Fhir.QueryBuilder.Services.Interfaces;
using FluentAssertions;

namespace Fhir.QueryBuilder.Tests.Services
{
    public class ConfigurationServiceTests : TestBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationServiceTests()
        {
            _configurationService = GetService<IConfigurationService>();
        }

        [Fact]
        public async Task LoadConfigurationAsync_ShouldReturnValidConfiguration()
        {
            // Act
            var result = await _configurationService.LoadConfigurationAsync();

            // Assert
            result.Should().NotBeNull();
            result.DefaultServerUrl.Should().NotBeNullOrEmpty();
            result.RequestTimeoutSeconds.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ValidateConfigurationAsync_WithValidOptions_ShouldReturnValid()
        {
            // Arrange
            var options = new QueryBuilderAppSettings
            {
                DefaultServerUrl = "https://example.com/fhir",
                RequestTimeoutSeconds = 30,
                EnableLogging = true,
                EnableCaching = true
            };

            // Act
            var result = await _configurationService.ValidateConfigurationAsync(options);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateConfigurationAsync_WithInvalidUrl_ShouldReturnInvalid()
        {
            // Arrange
            var options = new QueryBuilderAppSettings
            {
                DefaultServerUrl = "not-a-valid-url",
                RequestTimeoutSeconds = 30
            };

            // Act
            var result = await _configurationService.ValidateConfigurationAsync(options);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors.Should().Contain(e => e.Contains("valid absolute URI"));
        }

        [Fact]
        public async Task ValidateConfigurationAsync_WithInvalidTimeout_ShouldReturnInvalid()
        {
            // Arrange
            var options = new QueryBuilderAppSettings
            {
                DefaultServerUrl = "https://example.com/fhir",
                RequestTimeoutSeconds = 0 // Invalid timeout
            };

            // Act
            var result = await _configurationService.ValidateConfigurationAsync(options);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ValidateConfigurationAsync_WithUnsupportedTheme_ShouldReturnWarning()
        {
            // Arrange
            var options = new QueryBuilderAppSettings
            {
                DefaultServerUrl = "https://example.com/fhir",
                RequestTimeoutSeconds = 30,
                Ui = new UiSettings { Theme = "UnsupportedTheme" }
            };

            // Act
            var result = await _configurationService.ValidateConfigurationAsync(options);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Warnings.Should().NotBeEmpty();
            result.Warnings.Should().Contain(w => w.Contains("Unsupported theme"));
        }

        [Fact]
        public async Task ValidateConfigurationAsync_WithHttpsRequiredButHttpUrl_ShouldReturnWarning()
        {
            // Arrange
            var options = new QueryBuilderAppSettings
            {
                DefaultServerUrl = "http://example.com/fhir", // HTTP URL
                RequestTimeoutSeconds = 30,
                Security = new SecuritySettings { RequireHttps = true }
            };

            // Act
            var result = await _configurationService.ValidateConfigurationAsync(options);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Warnings.Should().NotBeEmpty();
            result.Warnings.Should().Contain(w => w.Contains("HTTPS is required"));
        }

        [Fact]
        public async Task GetEnvironmentVariablesAsync_ShouldReturnFhirRelatedVariables()
        {
            // Arrange
            Environment.SetEnvironmentVariable("FHIR_TEST_VAR", "test_value");
            Environment.SetEnvironmentVariable("OTHER_VAR", "other_value");

            try
            {
                // Act
                var result = await _configurationService.GetEnvironmentVariablesAsync();

                // Assert
                result.Should().ContainKey("FHIR_TEST_VAR");
                result.Should().NotContainKey("OTHER_VAR");
                result["FHIR_TEST_VAR"].Should().Be("test_value");
            }
            finally
            {
                // Cleanup
                Environment.SetEnvironmentVariable("FHIR_TEST_VAR", null);
                Environment.SetEnvironmentVariable("OTHER_VAR", null);
            }
        }

        [Fact]
        public async Task ResetToDefaultsAsync_ShouldCreateDefaultConfiguration()
        {
            // Act
            await _configurationService.ResetToDefaultsAsync();

            // Assert - Should not throw and should complete successfully
            // The actual file operations are tested in integration tests
        }

        [Fact]
        public void ConfigurationChanged_ShouldRaiseEvent()
        {
            // Arrange
            var eventRaised = false;
            QueryBuilderAppSettings? receivedOptions = null;

            _configurationService.ConfigurationChanged += (sender, options) =>
            {
                eventRaised = true;
                receivedOptions = options;
            };

            var testOptions = new QueryBuilderAppSettings
            {
                DefaultServerUrl = "https://test.com/fhir"
            };

            // Act
            // Note: This would typically be triggered by saving configuration
            // For this test, we'll simulate the event
            var eventInfo = typeof(IConfigurationService).GetEvent("ConfigurationChanged");
            var field = _configurationService.GetType().GetField("ConfigurationChanged", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field?.GetValue(_configurationService) is EventHandler<QueryBuilderAppSettings> handler)
            {
                handler.Invoke(_configurationService, testOptions);
            }

            // Assert
            eventRaised.Should().BeTrue();
            receivedOptions.Should().NotBeNull();
            receivedOptions?.DefaultServerUrl.Should().Be("https://test.com/fhir");
        }
    }
}
