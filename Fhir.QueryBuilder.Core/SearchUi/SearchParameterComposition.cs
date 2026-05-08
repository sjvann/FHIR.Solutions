using System.Text;

namespace Fhir.QueryBuilder.SearchUi;

/// <summary>
/// 將 WinForms NewMainForm Get*Parameter／GetGroup 邏輯抽成純函式（供 Avalonia／VM 呼叫）。
/// </summary>
public static class SearchParameterComposition
{
    /// <summary>對齊 WinForms GetGroup：無選中 modifier 時為 <c>=</c>，否則 <c>:{label}=</c>。</summary>
    public static string ModifierEqualsPrefix(string? modifierRadioLabel)
    {
        if (string.IsNullOrEmpty(modifierRadioLabel))
            return "=";
        return $":{modifierRadioLabel}=";
    }

    public static string ComposeStringSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.StringGroupModifier));
        sb.Append(d.StringValue);
        return sb.ToString();
    }

    public static string ComposeUriSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.UriGroupModifier));
        if (!string.IsNullOrEmpty(d.UriText))
            sb.Append(d.UriText);
        return sb.ToString();
    }

    /// <summary>Token 搜尋值本體（不含外層 <c>=</c>／modifier 前綴），亦用作 composite 元件片段。</summary>
    public static string ComposeTokenValueBody(string? codeOnly, string? system, string? codeWithSystem)
    {
        if (!string.IsNullOrEmpty(codeOnly))
            return codeOnly;
        if (!string.IsNullOrEmpty(codeWithSystem) && !string.IsNullOrEmpty(system))
            return $"{system}|{codeWithSystem}";
        if (!string.IsNullOrEmpty(system))
            return $"{system}|";
        return $"|{codeWithSystem}";
    }

    public static string ComposeTokenSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.TokenGroupModifier));
        sb.Append(ComposeTokenValueBody(d.TokenCodeOnly, d.TokenSystem, d.TokenCodeWithSystem));
        return sb.ToString();
    }

    public static string ComposeReferenceSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.ReferenceGroupModifier));

        if (!string.IsNullOrEmpty(d.ReferenceId))
        {
            sb.Append(d.ReferenceId);
        }
        else if (!string.IsNullOrEmpty(d.ReferenceType) && !string.IsNullOrEmpty(d.ReferenceTypeId))
        {
            sb.Append($"{d.ReferenceType}/{d.ReferenceTypeId}");
            if (!string.IsNullOrEmpty(d.ReferenceVersion))
                sb.Append($"/_history/{d.ReferenceVersion}");
        }
        else if (!string.IsNullOrEmpty(d.ReferenceUrl))
        {
            sb.Append(d.ReferenceUrl);
            if (!string.IsNullOrEmpty(d.ReferenceVersion))
                sb.Append($"|{d.ReferenceVersion}");
        }

        return sb.ToString();
    }

    public static string ComposeNumberSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.NumberGroupModifier));
        if (!string.IsNullOrEmpty(d.NumberPrefix))
            sb.Append(d.NumberPrefix);
        if (!string.IsNullOrEmpty(d.NumberValue))
            sb.Append(d.NumberValue);
        return sb.ToString();
    }

    public static string ComposeDateSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.DateGroupModifier));
        if (!string.IsNullOrEmpty(d.DatePrefix))
            sb.Append(d.DatePrefix);
        if (!string.IsNullOrEmpty(d.DateValue))
            sb.Append(d.DateValue);
        return sb.ToString();
    }

    /// <summary>Quantity 搜尋值本體（不含外層 modifier 前綴），亦用作 composite 元件片段。</summary>
    public static string ComposeQuantityValueBody(string? prefix, string? number, string? nscNumber, string? nscSystem, string? nscCode)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(prefix))
            sb.Append(prefix);
        if (!string.IsNullOrEmpty(number))
        {
            sb.Append(number);
            return sb.ToString();
        }

        if (!string.IsNullOrEmpty(nscNumber) && !string.IsNullOrEmpty(nscSystem) && !string.IsNullOrEmpty(nscCode))
        {
            sb.Append($"{nscNumber}|{nscSystem}|{nscCode}");
            return sb.ToString();
        }

        // 對齊舊版 WinForms：NSC 三方缺一時仍允許「number||code」片段；若三者皆空則不再附加無意义的「||」。
        if (!string.IsNullOrEmpty(nscNumber) || !string.IsNullOrEmpty(nscCode))
            sb.Append($"{nscNumber}||{nscCode}");

        return sb.ToString();
    }

    public static string ComposeQuantitySuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.QuantityGroupModifier));
        sb.Append(ComposeQuantityValueBody(
            d.QuantityPrefix,
            d.QuantityNumber,
            d.QuantityNscNumber,
            d.QuantityNscSystem,
            d.QuantityNscCode));
        return sb.ToString();
    }

    /// <summary>
    /// Composite：至少兩個逗號分隔元件，以 <c>$</c> 連接原始字串（不重複 per-component escape；最終 URL 由呼叫端對 value 編碼一次）。
    /// </summary>
    /// <returns>元件不足時為 <see langword="false"/>。</returns>
    public static bool TryBuildCompositeSuffix(TypedSearchDraft d, out string suffix)
    {
        if (d.CompositePartRows.Count >= 2)
        {
            var segments = new List<string>();
            foreach (var r in d.CompositePartRows)
            {
                if (!TryGetCompositePartSegment(r, out var seg) || string.IsNullOrEmpty(seg))
                {
                    suffix = string.Empty;
                    return false;
                }

                segments.Add(seg);
            }

            suffix = ModifierEqualsPrefix(d.CompositeGroupModifier) + string.Join('$', segments);
            return true;
        }

        var parts = d.CompositeComponentsCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(static s => s.Trim())
            .Where(static s => s.Length > 0)
            .ToArray();

        if (parts.Length < 2)
        {
            suffix = string.Empty;
            return false;
        }

        suffix = ModifierEqualsPrefix(d.CompositeGroupModifier) + string.Join('$', parts);
        return true;
    }

    /// <summary>供測試／UI 預覽：組出單一 composite 元件片段（不含 <c>$</c>）。</summary>
    public static bool TryGetCompositePartSegment(CompositePartRow r, out string segment)
    {
        var t = r.NormalizedComponentType.Trim().ToLowerInvariant();

        if (t == "token" && CompositeTokenRowHasInput(r))
        {
            segment = ComposeTokenValueBody(r.TokenCodeOnly, r.TokenSystem, r.TokenCodeWithSystem);
            return !string.IsNullOrEmpty(segment);
        }

        if (t == "quantity" && CompositeQuantityRowHasInput(r))
        {
            segment = ComposeQuantityValueBody(
                r.QuantityPrefix,
                r.QuantityNumber,
                r.QuantityNscNumber,
                r.QuantityNscSystem,
                r.QuantityNscCode);
            return !string.IsNullOrEmpty(segment);
        }

        if (t.Length > 0 && t is not "token" and not "quantity")
        {
            segment = r.Value.Trim();
            return segment.Length > 0;
        }

        segment = r.Value.Trim();
        return segment.Length > 0;
    }

    private static bool CompositeTokenRowHasInput(CompositePartRow r) =>
        !string.IsNullOrWhiteSpace(r.TokenCodeOnly)
        || !string.IsNullOrWhiteSpace(r.TokenSystem)
        || !string.IsNullOrWhiteSpace(r.TokenCodeWithSystem);

    private static bool CompositeQuantityRowHasInput(CompositePartRow r) =>
        !string.IsNullOrWhiteSpace(r.QuantityPrefix)
        || !string.IsNullOrWhiteSpace(r.QuantityNumber)
        || !string.IsNullOrWhiteSpace(r.QuantityNscNumber)
        || !string.IsNullOrWhiteSpace(r.QuantityNscSystem)
        || !string.IsNullOrWhiteSpace(r.QuantityNscCode);

    public static string ComposeSpecialSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.SpecialGroupModifier));
        sb.Append(d.SpecialValue);
        return sb.ToString();
    }

    /// <summary>依 Capability 回傳之 parameter type（小寫）組出值片段（含前置 <c>=</c>／modifier）。</summary>
    /// <remarks><c>composite</c> 請改用 <see cref="TryBuildCompositeSuffix"/>（需至少兩元件）。</remarks>
    public static string? ComposeSuffixForSearchParamType(string? searchParamType, TypedSearchDraft d)
    {
        return searchParamType?.Trim().ToLowerInvariant() switch
        {
            "string" => ComposeStringSuffix(d),
            "token" => ComposeTokenSuffix(d),
            "uri" => ComposeUriSuffix(d),
            "reference" => ComposeReferenceSuffix(d),
            "number" => ComposeNumberSuffix(d),
            "date" => ComposeDateSuffix(d),
            "quantity" => ComposeQuantitySuffix(d),
            "special" => ComposeSpecialSuffix(d),
            _ => null
        };
    }
}
