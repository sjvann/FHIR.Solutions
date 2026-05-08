using System.Text.Json;

namespace Fhir.Terminology.App;

/// <summary>從 FHIR OperationOutcome JSON 取出第一則 diagnostics（供匯入錯誤顯示）。</summary>
internal static class TerminologyImportOutcome
{
    public static string? TryGetDiagnostics(string jsonBody)
    {
        try
        {
            using var doc = JsonDocument.Parse(jsonBody);
            if (!doc.RootElement.TryGetProperty("issue", out var issues))
                return null;
            foreach (var issue in issues.EnumerateArray())
            {
                if (issue.TryGetProperty("diagnostics", out var d))
                    return d.GetString();
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }
}
