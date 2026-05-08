using Fhir.QueryBuilder.QueryBuilders;
using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Fhir.QueryBuilder.Tests.QueryBuilders
{
    /// <summary>
    /// Composite 參數建構器測試
    /// </summary>
    public class CompositeParameterBuilderTests : TestBase
    {
        public CompositeParameterBuilderTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CanHandle_ShouldReturnTrueForComposite()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<CompositeParameterBuilder>>();
            var builder = new CompositeParameterBuilder(logger);

            // Act & Assert
            Assert.True(builder.CanHandle("composite"));
            Assert.True(builder.CanHandle("COMPOSITE"));
            Assert.False(builder.CanHandle("string"));
        }

        [Fact]
        public void BuildParameter_ShouldCreateCorrectCompositeValue()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<CompositeParameterBuilder>>();
            var builder = new CompositeParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "component-code-value-quantity",
                ParameterType = "composite",
                Values = new Dictionary<string, object>
                {
                    { "components", new[] { "http://loinc.org|8480-6", "120", "mmHg" } }
                }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            Assert.Contains("component-code-value-quantity=", result);
            Assert.Contains("%24", result); // '$' 於 query 值經 EscapeDataString 後
            Assert.Contains("8480-6", result);
            Assert.Contains("120", result);
            Assert.Contains("mmHg", result);
            
            Output.WriteLine($"Built parameter: {result}");
        }

        [Fact]
        public void BuildParameter_WithModifier_ShouldIncludeModifier()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<CompositeParameterBuilder>>();
            var builder = new CompositeParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "component-code-value-quantity",
                ParameterType = "composite",
                Modifier = "missing",
                Values = new Dictionary<string, object>
                {
                    { "components", new[] { "true" } }
                }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            Assert.Contains("component-code-value-quantity:missing=", result);
            
            Output.WriteLine($"Built parameter with modifier: {result}");
        }

        [Fact]
        public void ValidateParameter_WithValidComponents_ShouldPass()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<CompositeParameterBuilder>>();
            var builder = new CompositeParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "component-code-value-quantity",
                ParameterType = "composite",
                Values = new Dictionary<string, object>
                {
                    { "components", new[] { "http://loinc.org|8480-6", "120" } }
                }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            
            Output.WriteLine($"Validation result: Valid={result.IsValid}, Errors={result.Errors.Count}");
        }

        [Fact]
        public void ValidateParameter_WithInsufficientComponents_ShouldFail()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<CompositeParameterBuilder>>();
            var builder = new CompositeParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "component-code-value-quantity",
                ParameterType = "composite",
                Values = new Dictionary<string, object>
                {
                    { "components", new[] { "single-component" } }
                }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains("at least 2 components", result.Errors[0]);
            
            Output.WriteLine($"Validation errors: {string.Join(", ", result.Errors)}");
        }

        [Fact]
        public void ValidateParameter_WithEmptyParameterName_ShouldFail()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<CompositeParameterBuilder>>();
            var builder = new CompositeParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "",
                ParameterType = "composite",
                Values = new Dictionary<string, object>
                {
                    { "components", new[] { "comp1", "comp2" } }
                }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Parameter name cannot be empty", result.Errors);
            
            Output.WriteLine($"Validation errors: {string.Join(", ", result.Errors)}");
        }

        [Theory]
        [InlineData("http://loinc.org", "8480-6", "120", "mmHg")]
        [InlineData("", "code123", "75.5", "")]
        [InlineData("http://snomed.info/sct", "271649006", "100", "mg")]
        public void CreateTokenQuantityComponents_ShouldBuildCorrectly(string system, string code, string value, string unit)
        {
            // Act
            var components = CompositeParameterExtensions.CreateTokenQuantityComponents(
                system, code, decimal.Parse(value), unit);

            // Assert
            Assert.Equal(2, components.Length);
            
            if (string.IsNullOrEmpty(system))
            {
                Assert.Equal(code, components[0]);
            }
            else
            {
                Assert.Equal($"{system}|{code}", components[0]);
            }
            
            if (string.IsNullOrEmpty(unit))
            {
                Assert.Equal(value, components[1]);
            }
            else
            {
                Assert.Contains(unit, components[1]);
            }
            
            Output.WriteLine($"Token+Quantity components: [{string.Join(", ", components)}]");
        }

        [Fact]
        public void CreateTokenDateComponents_ShouldBuildCorrectly()
        {
            // Arrange
            var system = "http://loinc.org";
            var code = "33747-0";
            var date = new DateTime(2023, 6, 15);

            // Act
            var components = CompositeParameterExtensions.CreateTokenDateComponents(system, code, date);

            // Assert
            Assert.Equal(2, components.Length);
            Assert.Equal($"{system}|{code}", components[0]);
            Assert.Equal("2023-06-15", components[1]);
            
            Output.WriteLine($"Token+Date components: [{string.Join(", ", components)}]");
        }

        [Fact]
        public void CreateStringComponents_ShouldBuildCorrectly()
        {
            // Act
            var components = CompositeParameterExtensions.CreateStringComponents(
                "first", "second", "third", "fourth");

            // Assert
            Assert.Equal(4, components.Length);
            Assert.Equal("first", components[0]);
            Assert.Equal("second", components[1]);
            Assert.Equal("third", components[2]);
            Assert.Equal("fourth", components[3]);
            
            Output.WriteLine($"String components: [{string.Join(", ", components)}]");
        }

        [Fact]
        public void CreateReferenceTokenComponents_ShouldBuildCorrectly()
        {
            // Arrange
            var reference = "Patient/123";
            var system = "http://loinc.org";
            var code = "8480-6";

            // Act
            var components = CompositeParameterExtensions.CreateReferenceTokenComponents(reference, system, code);

            // Assert
            Assert.Equal(2, components.Length);
            Assert.Equal(reference, components[0]);
            Assert.Equal($"{system}|{code}", components[1]);
            
            Output.WriteLine($"Reference+Token components: [{string.Join(", ", components)}]");
        }

        [Fact]
        public void Factory_ShouldReturnCompositeBuilder()
        {
            // Arrange
            var factory = ServiceProvider.GetRequiredService<ISearchParameterFactory>();

            // Act
            var builder = factory.GetBuilder("composite");

            // Assert
            Assert.NotNull(builder);
            Assert.IsType<CompositeParameterBuilder>(builder);
            Assert.True(builder.CanHandle("composite"));
            
            Output.WriteLine($"Factory returned: {builder.GetType().Name}");
        }

        [Fact]
        public void SupportedParameterTypes_ShouldIncludeComposite()
        {
            // Arrange
            var factory = ServiceProvider.GetRequiredService<ISearchParameterFactory>();

            // Act
            var supportedTypes = factory.GetSupportedParameterTypes();

            // Assert
            Assert.Contains("composite", supportedTypes);
            
            Output.WriteLine($"Supported types: {string.Join(", ", supportedTypes)}");
        }
    }
}
