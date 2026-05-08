using Fhir.QueryBuilder.QueryBuilders;
using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Fhir.QueryBuilder.Tests.QueryBuilders
{
    /// <summary>
    /// Filter 參數建構器測試
    /// </summary>
    public class FilterParameterBuilderTests : TestBase
    {
        public FilterParameterBuilderTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CanHandle_ShouldReturnTrueForFilter()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<FilterParameterBuilder>>();
            var builder = new FilterParameterBuilder(logger);

            // Act & Assert
            Assert.True(builder.CanHandle("filter"));
            Assert.True(builder.CanHandle("FILTER"));
            Assert.False(builder.CanHandle("string"));
        }

        [Fact]
        public void BuildParameter_ShouldCreateCorrectFilterValue()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<FilterParameterBuilder>>();
            var builder = new FilterParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "_filter",
                ParameterType = "filter",
                Values = new Dictionary<string, object>
                {
                    { "expression", "name co 'John' and birthDate ge 1990-01-01" }
                }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            Assert.StartsWith("_filter=", result);
            Assert.Contains("John", result);
            Assert.Contains("1990-01-01", result);
            
            Output.WriteLine($"Built filter parameter: {result}");
        }

        [Fact]
        public void ValidateParameter_WithValidExpression_ShouldPass()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<FilterParameterBuilder>>();
            var builder = new FilterParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "_filter",
                ParameterType = "filter",
                Values = new Dictionary<string, object>
                {
                    { "expression", "name eq 'John'" }
                }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            
            Output.WriteLine($"Validation result: Valid={result.IsValid}");
        }

        [Fact]
        public void ValidateParameter_WithEmptyExpression_ShouldFail()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<FilterParameterBuilder>>();
            var builder = new FilterParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "_filter",
                ParameterType = "filter",
                Values = new Dictionary<string, object>
                {
                    { "expression", "" }
                }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Filter expression cannot be empty", result.Errors);
            
            Output.WriteLine($"Validation errors: {string.Join(", ", result.Errors)}");
        }

        [Fact]
        public void ValidateParameter_WithUnbalancedParentheses_ShouldFail()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<FilterParameterBuilder>>();
            var builder = new FilterParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "_filter",
                ParameterType = "filter",
                Values = new Dictionary<string, object>
                {
                    { "expression", "(name eq 'John' and (birthDate ge 1990" }
                }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Unbalanced parentheses", result.Errors[0]);
            
            Output.WriteLine($"Validation errors: {string.Join(", ", result.Errors)}");
        }

        [Fact]
        public void ValidateParameter_WithUnbalancedQuotes_ShouldFail()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<FilterParameterBuilder>>();
            var builder = new FilterParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "_filter",
                ParameterType = "filter",
                Values = new Dictionary<string, object>
                {
                    { "expression", "name eq 'John and age gt 30" }
                }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Unbalanced quotes", result.Errors[0]);
            
            Output.WriteLine($"Validation errors: {string.Join(", ", result.Errors)}");
        }

        [Theory]
        [InlineData("name", "John", "name eq 'John'")]
        [InlineData("age", "30", "age eq '30'")]
        [InlineData("status", "active", "status eq 'active'")]
        public void CreateEqualFilter_ShouldBuildCorrectly(string path, string value, string expected)
        {
            // Act
            var result = FilterParameterExtensions.CreateEqualFilter(path, value);

            // Assert
            Assert.Equal(expected, result);
            
            Output.WriteLine($"Equal filter: {result}");
        }

        [Theory]
        [InlineData("name", "John", "name co 'John'")]
        [InlineData("description", "test", "description co 'test'")]
        public void CreateContainsFilter_ShouldBuildCorrectly(string path, string value, string expected)
        {
            // Act
            var result = FilterParameterExtensions.CreateContainsFilter(path, value);

            // Assert
            Assert.Equal(expected, result);
            
            Output.WriteLine($"Contains filter: {result}");
        }

        [Fact]
        public void CreateRangeFilter_ShouldBuildCorrectly()
        {
            // Arrange
            var path = "birthDate";
            var minValue = "1990-01-01";
            var maxValue = "2000-12-31";

            // Act
            var result = FilterParameterExtensions.CreateRangeFilter(path, minValue, maxValue);

            // Assert
            Assert.Contains("ge '1990-01-01'", result);
            Assert.Contains("le '2000-12-31'", result);
            Assert.Contains("and", result);
            
            Output.WriteLine($"Range filter: {result}");
        }

        [Fact]
        public void CreateOrFilter_ShouldBuildCorrectly()
        {
            // Arrange
            var conditions = new[]
            {
                "name eq 'John'",
                "name eq 'Jane'",
                "name eq 'Bob'"
            };

            // Act
            var result = FilterParameterExtensions.CreateOrFilter(conditions);

            // Assert
            Assert.StartsWith("(", result);
            Assert.EndsWith(")", result);
            Assert.Contains(" or ", result);
            Assert.Contains("John", result);
            Assert.Contains("Jane", result);
            Assert.Contains("Bob", result);
            
            Output.WriteLine($"OR filter: {result}");
        }

        [Fact]
        public void CreateAndFilter_ShouldBuildCorrectly()
        {
            // Arrange
            var conditions = new[]
            {
                "name eq 'John'",
                "age gt 18",
                "status eq 'active'"
            };

            // Act
            var result = FilterParameterExtensions.CreateAndFilter(conditions);

            // Assert
            Assert.Contains(" and ", result);
            Assert.Contains("John", result);
            Assert.Contains("age gt 18", result);
            Assert.Contains("active", result);
            
            Output.WriteLine($"AND filter: {result}");
        }

        [Fact]
        public void CreateNotFilter_ShouldBuildCorrectly()
        {
            // Arrange
            var condition = "name eq 'John'";

            // Act
            var result = FilterParameterExtensions.CreateNotFilter(condition);

            // Assert
            Assert.StartsWith("not (", result);
            Assert.EndsWith(")", result);
            Assert.Contains("John", result);
            
            Output.WriteLine($"NOT filter: {result}");
        }

        [Theory]
        [InlineData("name", true, "name ne null")]
        [InlineData("description", false, "description eq null")]
        public void CreateExistsFilter_ShouldBuildCorrectly(string path, bool exists, string expected)
        {
            // Act
            var result = FilterParameterExtensions.CreateExistsFilter(path, exists);

            // Assert
            Assert.Equal(expected, result);
            
            Output.WriteLine($"Exists filter: {result}");
        }

        [Fact]
        public void Factory_ShouldReturnFilterBuilder()
        {
            // Arrange
            var factory = ServiceProvider.GetRequiredService<ISearchParameterFactory>();

            // Act
            var builder = factory.GetBuilder("filter");

            // Assert
            Assert.NotNull(builder);
            Assert.IsType<FilterParameterBuilder>(builder);
            Assert.True(builder.CanHandle("filter"));
            
            Output.WriteLine($"Factory returned: {builder.GetType().Name}");
        }

        [Fact]
        public void SupportedParameterTypes_ShouldIncludeFilter()
        {
            // Arrange
            var factory = ServiceProvider.GetRequiredService<ISearchParameterFactory>();

            // Act
            var supportedTypes = factory.GetSupportedParameterTypes();

            // Assert
            Assert.Contains("filter", supportedTypes);
            
            Output.WriteLine($"Supported types: {string.Join(", ", supportedTypes)}");
        }

        [Fact]
        public void BuildFromContext_ShouldCreateExpressionFromValues()
        {
            // Arrange
            var logger = ServiceProvider.GetRequiredService<ILogger<FilterParameterBuilder>>();
            var builder = new FilterParameterBuilder(logger);
            
            var context = new SearchParameterContext
            {
                ParameterName = "name",
                ParameterType = "filter",
                Values = new Dictionary<string, object>
                {
                    { "equal", "John" },
                    { "path", "name" }
                }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            Assert.Contains("_filter=", result);
            Assert.Contains("name", result);
            Assert.Contains("John", result);
            
            Output.WriteLine($"Built from context: {result}");
        }
    }
}
