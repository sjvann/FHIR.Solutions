namespace Fhir.QueryBuilder.Platform;

/// <summary>擷取 SMART-on-FHIR .well-known/smart-configuration（取代外部 TokenServices）。</summary>
internal static class WellKnownSmartConfig
{
    public static async Task<string> FetchAsync(string baseUrl, CancellationToken cancellationToken = default)
    {
        var url = $"{baseUrl.TrimEnd('/')}/.well-known/smart-configuration";
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            return await client.GetStringAsync(new Uri(url), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return $"{{ \"error\": \"{ex.Message.Replace("\"", "\\\"")}\" }}";
        }
    }
}
