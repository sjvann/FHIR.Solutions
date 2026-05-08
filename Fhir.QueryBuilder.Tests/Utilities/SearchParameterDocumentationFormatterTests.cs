using Fhir.QueryBuilder.Utilities;
using FluentAssertions;

namespace Fhir.QueryBuilder.Tests.Utilities;

public class SearchParameterDocumentationFormatterTests
{
    [Fact]
    public void ToPlain_RemovesTagsAndEscapes()
    {
        var raw = """Gender of the patient\<br /> \<em>\> NOTE\</em>\>: See \<a href="http://x"\>link\</a>.""";
        var plain = SearchParameterDocumentationFormatter.ToPlain(raw);

        plain.Should().NotContain("<");
        plain.Should().NotContain("\\<");
        plain.Should().Contain("Gender");
        plain.Should().Contain("NOTE");
    }

    [Fact]
    public void ToPlain_DecodesEntitiesAndCollapsesSpace()
    {
        var raw = "a&nbsp;&nbsp;b  <span>c</span>";
        SearchParameterDocumentationFormatter.ToPlain(raw).Should().Be("a b c");
    }
}
