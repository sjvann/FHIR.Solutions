using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace Fhir.QueryBuilder.SearchUi;

/// <summary>將 JSON 文字或 <see cref="JsonNode"/> 轉成樹狀節點（對齊 WinForms <c>TreeViewExtension.InitFromJsonNode</c>）。</summary>
public static class JsonTreeBuilder
{
    public static ObservableCollection<JsonTreeItem> BuildRootsFromJsonText(string? jsonText)
    {
        var roots = new ObservableCollection<JsonTreeItem>();
        if (string.IsNullOrWhiteSpace(jsonText))
            return roots;

        var trimmed = jsonText.TrimStart();
        if (!trimmed.StartsWith('{') && !trimmed.StartsWith('['))
            return roots;

        try
        {
            var node = JsonNode.Parse(jsonText);
            if (node == null)
                return roots;

            if (node is JsonArray arr)
            {
                var root = new JsonTreeItem { Header = "JSON" };
                AppendArrayChildren(root, arr);
                roots.Add(root);
                return roots;
            }

            var rootObj = new JsonTreeItem
            {
                Header = node["resourceType"]?.GetValue<string>() ?? "JSON"
            };
            AppendChildren(rootObj, node);
            roots.Add(rootObj);
        }
        catch
        {
            // 無效 JSON：維持空樹，由 UI 留在文字檢視
        }

        return roots;
    }

    private static void AppendChildren(JsonTreeItem parent, JsonNode? jsonNode)
    {
        if (jsonNode is JsonObject jObject)
        {
            foreach (var item in jObject)
            {
                if (item.Value is JsonValue jValue)
                {
                    parent.Children.Add(new JsonTreeItem
                    {
                        Header = $"{item.Key}: {FormatScalar(jValue)}"
                    });
                }
                else
                {
                    var sub = new JsonTreeItem { Header = item.Key };
                    parent.Children.Add(sub);
                    if (item.Value is JsonObject subObject)
                        AppendChildren(sub, subObject);
                    else if (item.Value is JsonArray jArray)
                        AppendArrayChildren(sub, jArray);
                }
            }
        }
    }

    private static void AppendArrayChildren(JsonTreeItem parent, JsonArray jArray)
    {
        var index = 0;
        foreach (var subItem in jArray)
        {
            if (subItem is JsonValue jv)
            {
                parent.Children.Add(new JsonTreeItem { Header = $"{index}: {FormatScalar(jv)}" });
            }
            else
            {
                var subArrayNode = new JsonTreeItem { Header = index.ToString() };
                parent.Children.Add(subArrayNode);
                AppendChildren(subArrayNode, subItem);
            }

            index++;
        }
    }

    private static string FormatScalar(JsonValue source)
    {
        if (source.TryGetValue<string>(out var str))
            return string.IsNullOrEmpty(str) ? string.Empty : $"\"{str}\"";

        if (source.TryGetValue<DateTime>(out var dt))
            return $"\"{dt}\"";

        return source.ToJsonString();
    }
}
