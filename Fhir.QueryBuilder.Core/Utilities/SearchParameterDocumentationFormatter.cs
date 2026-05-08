using System.Net;
using System.Text.RegularExpressions;

namespace Fhir.QueryBuilder.Utilities;

/// <summary>
/// CapabilityStatement searchParam.documentation 常含 XHTML／連結；UI 顯示為純文字以避免標籤與逸出字元噪音。
/// </summary>
public static partial class SearchParameterDocumentationFormatter
{
    /// <summary>取得適合一般文字區塊顯示的說明（無 HTML 標籤）。</summary>
    public static string ToPlain(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";

        var s = raw.Trim();

        // 部分序列化來源會在前綴加上反斜線
        s = s.Replace("\\<", "<", StringComparison.Ordinal).Replace("\\>", ">", StringComparison.Ordinal);

        s = WebUtility.HtmlDecode(s);

        s = SpaceTagsRegex().Replace(s, " ");
        s = StripTagsRegex().Replace(s, " ");

        s = WebUtility.HtmlDecode(s);
        s = WhiteSpaceCollapseRegex().Replace(s, " ").Trim();

        return s;
    }

    [GeneratedRegex(@"<br\s*/?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SpaceTagsRegex();

    [GeneratedRegex(@"<[^>]+>", RegexOptions.CultureInvariant)]
    private static partial Regex StripTagsRegex();

    [GeneratedRegex(@"\s+", RegexOptions.CultureInvariant)]
    private static partial Regex WhiteSpaceCollapseRegex();
}
