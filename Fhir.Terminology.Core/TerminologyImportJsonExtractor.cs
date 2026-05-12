using System.Text.Json;

namespace Fhir.Terminology.Core;

/// <summary>
/// 自貼上／上傳的 FHIR JSON 或 NPM 套件內檔案，取出可寫入資源。
/// 根節點為單筆時：CodeSystem／ValueSet／ConceptMap／StructureDefinition（須含 <c>id</c>）。
/// 根節點為 <c>Bundle</c> 時：擷取各 <c>entry.resource</c> 中為上述型別且含 <c>id</c> 者。
/// HL7 FHIR 規格發行通常將 <c>valuesets.json</c>（多為 CodeSystem／ValueSet）與 <c>conceptmaps.json</c>（僅 ConceptMap）分開檔案；同一機制分別套用在各檔即可。
/// </summary>
public static class TerminologyImportJsonExtractor
{
    /// <summary>
    /// 將 JSON 本文拆解為零至多筆可寫入資料庫的資源 JSON（每筆須含 <c>resourceType</c> 與 <c>id</c>）。
    /// </summary>
    public static IReadOnlyList<string> ExtractImportablePieces(string json, out string? skipReason)
    {
        skipReason = null;
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            if (!root.TryGetProperty("resourceType", out var rtEl))
            {
                skipReason = "no resourceType";
                return [];
            }

            var rt = rtEl.GetString();
            if (rt == "Bundle")
                return ExtractFromBundle(root, out skipReason);

            if (rt is not ("CodeSystem" or "ValueSet" or "ConceptMap" or "StructureDefinition"))
            {
                skipReason = "resourceType " + rt;
                return [];
            }

            if (!root.TryGetProperty("id", out var idEl) || string.IsNullOrEmpty(idEl.GetString()))
            {
                skipReason = "missing id";
                return [];
            }

            return [root.GetRawText()];
        }
        catch (JsonException ex)
        {
            skipReason = ex.Message;
            return [];
        }
    }

    private static IReadOnlyList<string> ExtractFromBundle(JsonElement bundleRoot, out string? skipReason)
    {
        skipReason = null;
        if (!bundleRoot.TryGetProperty("entry", out var entries) || entries.ValueKind != JsonValueKind.Array)
        {
            skipReason = "bundle without entry array";
            return [];
        }

        var list = new List<string>();
        foreach (var entry in entries.EnumerateArray())
        {
            if (!entry.TryGetProperty("resource", out var res))
                continue;
            if (!TryAcceptBundleTerminologyResource(res, out var piece))
                continue;
            list.Add(piece);
        }

        if (list.Count == 0)
            skipReason = "bundle had no CodeSystem/ValueSet/ConceptMap/StructureDefinition entries with id";

        return list;
    }

    private static bool TryAcceptBundleTerminologyResource(JsonElement resourceRoot, out string json)
    {
        json = "";
        if (!resourceRoot.TryGetProperty("resourceType", out var rtEl))
            return false;

        var rt = rtEl.GetString();
        if (rt is not ("CodeSystem" or "ValueSet" or "ConceptMap" or "StructureDefinition"))
            return false;

        if (!resourceRoot.TryGetProperty("id", out var idEl) || string.IsNullOrEmpty(idEl.GetString()))
            return false;

        json = resourceRoot.GetRawText();
        return true;
    }
}
