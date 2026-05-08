using Fhir.QueryBuilder.SearchUi;
using FluentAssertions;

namespace Fhir.QueryBuilder.Tests.SearchUi;

public class SearchParameterCompositionTests
{
    [Fact]
    public void TryBuildCompositeSuffix_WithTwoComponents_JoinsWithDollar()
    {
        var d = new TypedSearchDraft { CompositeComponentsCsv = "  a , b " };
        SearchParameterComposition.TryBuildCompositeSuffix(d, out var suffix).Should().BeTrue();
        suffix.Should().Be("=a$b");
    }

    [Fact]
    public void TryBuildCompositeSuffix_WithModifier_PrefixesModifierEquals()
    {
        var d = new TypedSearchDraft
        {
            CompositeGroupModifier = "missing",
            CompositeComponentsCsv = "x,y"
        };
        SearchParameterComposition.TryBuildCompositeSuffix(d, out var suffix).Should().BeTrue();
        suffix.Should().Be(":missing=x$y");
    }

    [Fact]
    public void TryBuildCompositeSuffix_WithOneComponent_ReturnsFalse()
    {
        var d = new TypedSearchDraft { CompositeComponentsCsv = "onlyone" };
        SearchParameterComposition.TryBuildCompositeSuffix(d, out _).Should().BeFalse();
    }

    [Fact]
    public void ComposeSpecialSuffix_MatchesStringShape()
    {
        var d = new TypedSearchDraft { SpecialGroupModifier = "exact", SpecialValue = "foo|bar" };
        SearchParameterComposition.ComposeSpecialSuffix(d).Should().Be(":exact=foo|bar");
    }

    [Fact]
    public void ComposeSuffixForSearchParamType_Special_DelegatesToComposeSpecialSuffix()
    {
        var d = new TypedSearchDraft { SpecialValue = "v" };
        SearchParameterComposition.ComposeSuffixForSearchParamType("special", d).Should().Be("=v");
    }

    [Fact]
    public void ComposeSuffixForSearchParamType_Composite_IsNull_UseTryBuild()
    {
        var d = new TypedSearchDraft { CompositeComponentsCsv = "a,b" };
        SearchParameterComposition.ComposeSuffixForSearchParamType("composite", d).Should().BeNull();
    }

    [Fact]
    public void TryBuildCompositeSuffix_UsesPartRowsOverCsvWhenAllFilled()
    {
        var d = new TypedSearchDraft { CompositeComponentsCsv = "x,y,z" };
        d.CompositePartRows.Add(new CompositePartRow { Value = "a" });
        d.CompositePartRows.Add(new CompositePartRow { Value = "b" });
        SearchParameterComposition.TryBuildCompositeSuffix(d, out var suffix).Should().BeTrue();
        suffix.Should().Be("=a$b");
    }

    [Fact]
    public void TryBuildCompositeSuffix_WithTypedTokenAndQuantity_JoinsSegmentsWithDollar()
    {
        var d = new TypedSearchDraft();
        d.CompositePartRows.Add(new CompositePartRow
        {
            NormalizedComponentType = "token",
            TokenSystem = "http://loinc.org",
            TokenCodeWithSystem = "8480-6"
        });
        d.CompositePartRows.Add(new CompositePartRow
        {
            NormalizedComponentType = "quantity",
            QuantityPrefix = "lt",
            QuantityNumber = "60"
        });
        SearchParameterComposition.TryBuildCompositeSuffix(d, out var suffix).Should().BeTrue();
        suffix.Should().Be("=http://loinc.org|8480-6$lt60");
    }

    [Fact]
    public void ComposeQuantityValueBody_PrefixAndNumber_DoesNotAppendEmptyNscTail()
    {
        SearchParameterComposition.ComposeQuantityValueBody("lt", "60", "", "", "")
            .Should().Be("lt60");
    }

    [Fact]
    public void ComposeQuantityValueBody_PrefixOnlyWithoutNsc_DoesNotAppendDoublePipe()
    {
        SearchParameterComposition.ComposeQuantityValueBody("lt", "", "", "", "")
            .Should().Be("lt");
    }

    [Fact]
    public void ComposeQuantityValueBody_AllEmpty_IsEmptyString()
    {
        SearchParameterComposition.ComposeQuantityValueBody("", "", "", "", "")
            .Should().BeEmpty();
    }
}
