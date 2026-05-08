using Fhir.QueryBuilder.QueryBuilders.Advanced;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fhir.QueryBuilder.Tests.QueryBuilders;

/// <summary>TD-2：composite 值應只做一次 query-value 層級編碼。</summary>
public class CompositeEncodingTests
{
    [Fact]
    public void AdvancedSearchParameterBuilder_AddCompositeParameter_Build_EncodesJoinedValueOnce()
    {
        var registry = new Mock<ISearchParameterRegistry>();
        var logger = new Mock<ILogger<AdvancedSearchParameterBuilder>>();
        var b = new AdvancedSearchParameterBuilder(registry.Object, logger.Object);

        b.AddCompositeParameter("combo", "a|b", "x y");

        var q = b.Build();
        var expectedValue = Uri.EscapeDataString("a|b$x y");
        q.Should().Be($"combo={expectedValue}");
    }

    [Fact]
    public void AdvancedSearchParameterBuilder_Composite_DoesNotDoubleEncodePercent()
    {
        var registry = new Mock<ISearchParameterRegistry>();
        var logger = new Mock<ILogger<AdvancedSearchParameterBuilder>>();
        var b = new AdvancedSearchParameterBuilder(registry.Object, logger.Object);

        b.AddCompositeParameter("c", "100%", "200%");

        var q = b.Build();
        q.Should().NotContain("%2525", "double-encoding would turn % into %25 twice");
        q.Should().Contain(Uri.EscapeDataString("100%$200%"));
    }
}
