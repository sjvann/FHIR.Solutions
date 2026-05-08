using Fhir.QueryBuilder.QueryBuilders;
using Fhir.QueryBuilder.QueryBuilders.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fhir.QueryBuilder.Tests.QueryBuilders
{
    public class SearchParameterBuilderTests
    {
        private readonly Mock<ILogger<StringParameterBuilder>> _mockLogger;

        public SearchParameterBuilderTests()
        {
            _mockLogger = new Mock<ILogger<StringParameterBuilder>>();
        }

        [Fact]
        public void StringParameterBuilder_CanHandle_ShouldReturnTrueForStringType()
        {
            // Arrange
            var builder = new StringParameterBuilder(_mockLogger.Object);

            // Act
            var result = builder.CanHandle("string");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void StringParameterBuilder_CanHandle_ShouldReturnFalseForOtherTypes()
        {
            // Arrange
            var builder = new StringParameterBuilder(_mockLogger.Object);

            // Act
            var result = builder.CanHandle("date");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void StringParameterBuilder_BuildParameter_ShouldGenerateCorrectParameter()
        {
            // Arrange
            var builder = new StringParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "family",
                ParameterType = "string",
                Values = new Dictionary<string, object> { { "value", "Smith" } }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            result.Should().Be("family=Smith");
        }

        [Fact]
        public void StringParameterBuilder_BuildParameter_WithModifier_ShouldGenerateCorrectParameter()
        {
            // Arrange
            var builder = new StringParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "family",
                ParameterType = "string",
                Values = new Dictionary<string, object> { { "value", "Smith" } },
                Modifier = "exact"
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            result.Should().Be("family:exact=Smith");
        }

        [Fact]
        public void StringParameterBuilder_BuildParameter_WithEmptyValue_ShouldThrowException()
        {
            // Arrange
            var builder = new StringParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "family",
                ParameterType = "string",
                Values = new Dictionary<string, object> { { "value", "" } }
            };

            // Act & Assert
            var action = () => builder.BuildParameter(context);
            action.Should().Throw<ArgumentException>()
                .WithMessage("String parameter value cannot be empty");
        }

        [Fact]
        public void StringParameterBuilder_ValidateParameter_WithValidInput_ShouldReturnValid()
        {
            // Arrange
            var builder = new StringParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "family",
                ParameterType = "string",
                Values = new Dictionary<string, object> { { "value", "Smith" } }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void StringParameterBuilder_ValidateParameter_WithLongValue_ShouldReturnWarning()
        {
            // Arrange
            var builder = new StringParameterBuilder(_mockLogger.Object);
            var longValue = new string('a', 1500); // Very long string
            var context = new SearchParameterContext
            {
                ParameterName = "family",
                ParameterType = "string",
                Values = new Dictionary<string, object> { { "value", longValue } }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Warnings.Should().NotBeEmpty();
            result.Warnings.Should().Contain(w => w.Contains("very long"));
        }
    }

    public class DateParameterBuilderTests
    {
        private readonly Mock<ILogger<DateParameterBuilder>> _mockLogger;

        public DateParameterBuilderTests()
        {
            _mockLogger = new Mock<ILogger<DateParameterBuilder>>();
        }

        [Theory]
        [InlineData("2023-01-01", true)]
        [InlineData("2023-01", true)]
        [InlineData("2023", true)]
        [InlineData("invalid-date", false)]
        [InlineData("", false)]
        public void DateParameterBuilder_ValidateParameter_ShouldReturnExpectedResult(string dateValue, bool expectedValid)
        {
            // Arrange
            var builder = new DateParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "birthdate",
                ParameterType = "date",
                Values = new Dictionary<string, object> { { "value", dateValue } }
            };

            // Act
            var result = builder.ValidateParameter(context);

            // Assert
            result.IsValid.Should().Be(expectedValid);
        }

        [Fact]
        public void DateParameterBuilder_BuildParameter_WithPrefix_ShouldGenerateCorrectParameter()
        {
            // Arrange
            var builder = new DateParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "birthdate",
                ParameterType = "date",
                Values = new Dictionary<string, object> 
                { 
                    { "value", "1990-01-01" },
                    { "prefix", "ge" }
                }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            result.Should().Be("birthdate=ge1990-01-01");
        }
    }

    public class TokenParameterBuilderTests
    {
        private readonly Mock<ILogger<TokenParameterBuilder>> _mockLogger;

        public TokenParameterBuilderTests()
        {
            _mockLogger = new Mock<ILogger<TokenParameterBuilder>>();
        }

        [Fact]
        public void TokenParameterBuilder_BuildParameter_WithSystemAndCode_ShouldGenerateCorrectParameter()
        {
            // Arrange
            var builder = new TokenParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "gender",
                ParameterType = "token",
                Values = new Dictionary<string, object> 
                { 
                    { "code", "male" },
                    { "system", "http://hl7.org/fhir/administrative-gender" }
                }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            result.Should().Be("gender=http://hl7.org/fhir/administrative-gender|male");
        }

        [Fact]
        public void TokenParameterBuilder_BuildParameter_WithCodeOnly_ShouldGenerateCorrectParameter()
        {
            // Arrange
            var builder = new TokenParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "gender",
                ParameterType = "token",
                Values = new Dictionary<string, object> { { "code", "male" } }
            };

            // Act
            var result = builder.BuildParameter(context);

            // Assert
            result.Should().Be("gender=male");
        }

        [Fact]
        public void TokenParameterBuilder_BuildParameter_WithoutCodeOrSystem_ShouldThrowException()
        {
            // Arrange
            var builder = new TokenParameterBuilder(_mockLogger.Object);
            var context = new SearchParameterContext
            {
                ParameterName = "gender",
                ParameterType = "token",
                Values = new Dictionary<string, object>()
            };

            // Act & Assert
            var action = () => builder.BuildParameter(context);
            action.Should().Throw<ArgumentException>()
                .WithMessage("Token parameter must have at least code or system");
        }
    }
}
