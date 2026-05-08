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

    public static string ComposeTokenSuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.TokenGroupModifier));

        if (!string.IsNullOrEmpty(d.TokenCodeOnly))
        {
            sb.Append(d.TokenCodeOnly);
        }
        else if (!string.IsNullOrEmpty(d.TokenCodeWithSystem) && !string.IsNullOrEmpty(d.TokenSystem))
        {
            sb.Append($"{d.TokenSystem}|{d.TokenCodeWithSystem}");
        }
        else if (!string.IsNullOrEmpty(d.TokenSystem))
        {
            sb.Append($"{d.TokenSystem}|");
        }
        else
        {
            sb.Append($"|{d.TokenCodeWithSystem}");
        }

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

    public static string ComposeQuantitySuffix(TypedSearchDraft d)
    {
        var sb = new StringBuilder();
        sb.Append(ModifierEqualsPrefix(d.QuantityGroupModifier));
        if (!string.IsNullOrEmpty(d.QuantityPrefix))
            sb.Append(d.QuantityPrefix);
        if (!string.IsNullOrEmpty(d.QuantityNumber))
        {
            sb.Append(d.QuantityNumber);
        }
        else if (!string.IsNullOrEmpty(d.QuantityNscNumber) && !string.IsNullOrEmpty(d.QuantityNscSystem) && !string.IsNullOrEmpty(d.QuantityNscCode))
        {
            sb.Append($"{d.QuantityNscNumber}|{d.QuantityNscSystem}|{d.QuantityNscCode}");
        }
        else
        {
            sb.Append($"{d.QuantityNscNumber}||{d.QuantityNscCode}");
        }

        return sb.ToString();
    }

    /// <summary>依 Capability 回傳之 parameter type（小寫）組出值片段（含前置 <c>=</c>／modifier）。</summary>
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
            _ => null
        };
    }
}
