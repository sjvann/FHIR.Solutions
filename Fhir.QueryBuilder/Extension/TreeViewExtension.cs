using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Fhir.QueryBuilder.Extension
{
    public static class TreeViewExtension
    {
        public static void InitFromJsonNode(this TreeView control, JsonNode? source)
        {
            if (source == null) return;
            string rootNodeName = source["resourceType"]?.GetValue<string>() ?? "JSON";
            TreeNode currentNode = control.Nodes.Add(rootNodeName);
            NewNode(ref currentNode, source);
        }
        //public static void InitFromJsonNode(this TreeView control, List<KeyValuePair<string, JsonNode?>?  source)
        //{
        //    if (source == null || source.Count == 0) return;
        //    control.Name = from item in source
        //                   where item. source.Find(x => x.ha)
        //    control.Nodes.Add("JSON");
        //    control.Nodes.Add(NewNode(source));
        //}
        private static void NewNode(ref TreeNode currentNode, JsonNode? jsonNode)
        {  
            if (jsonNode is JsonObject jObject)
            {
                foreach (var item in jObject)
                {
                    if (item.Value is JsonValue jValue)
                    {
                        currentNode.Nodes.Add($"{item.Key}: {CheckNodeType(jValue)}");
                    }
                    else
                    {
                        TreeNode subNode = currentNode.Nodes.Add(item.Key);
                        if (item.Value is JsonObject subJObject)
                        {
                            NewNode(ref subNode, subJObject);
                        }
                        else if (item.Value is JsonArray jArray)
                        {
                            int index = 0;
                            foreach (var subItem in jArray)
                            {
                                TreeNode subArrayNode = subNode.Nodes.Add(index.ToString());
                                NewNode(ref subArrayNode, subItem);
                                index++;
                            }
                        }
                    }
                }
            }
        }

        private static string CheckNodeType(JsonValue source)
        {
            if (source.TryGetValue<string>(out string? value))
            {
                return string.IsNullOrEmpty(value) ? string.Empty : $"\"{value}\"";
            }
            else if (source.TryGetValue<DateTime>(out DateTime valueDateTime))
            {
                return $"\"{valueDateTime.ToString()}\"";
            }
            else
            {
                return source.ToString();
            }

        }
    }
}
