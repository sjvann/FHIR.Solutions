using System.Net;
using System.Net.Http;
using Fhir.Terminology.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fhir.Terminology.Infrastructure;

public class RemoteTerminologyGateway(IHttpClientFactory httpFactory, TerminologyDbContext db, ILogger<RemoteTerminologyGateway> logger) : IRemoteTerminologyGateway
{
    public const string ClientName = "terminology-upstream";
    private readonly IHttpClientFactory _httpFactory = httpFactory;
    private readonly TerminologyDbContext _db = db;
    private readonly ILogger<RemoteTerminologyGateway> _logger = logger;

    public async Task<RemoteForwardResult?> ForwardAsync(string relativePathAndQuery, HttpMethod method, string? jsonBody, CancellationToken cancellationToken = default)
    {
        var upstreams = await _db.UpstreamServers.AsNoTracking()
            .OrderByDescending(x => x.SystemUriPrefix.Length)
            .ToListAsync(cancellationToken);
        if (upstreams.Count == 0)
            return null;

        var system = TryGetQueryParam(relativePathAndQuery, "system");
        UpstreamServerEntity? pick = null;
        if (!string.IsNullOrEmpty(system))
            pick = upstreams.FirstOrDefault(u => system.StartsWith(u.SystemUriPrefix, StringComparison.Ordinal));
        else
            pick = upstreams[0];

        if (pick is null)
            return null;

        var client = _httpFactory.CreateClient(ClientName);
        client.Timeout = TimeSpan.FromSeconds(Math.Max(1, pick.TimeoutSeconds));
        var fullUrl = pick.BaseUrl.TrimEnd('/') + "/" + relativePathAndQuery.TrimStart('/');

        using var req = new HttpRequestMessage(method, fullUrl);
        if (!string.IsNullOrEmpty(pick.AuthorizationHeader))
            req.Headers.TryAddWithoutValidation("Authorization", pick.AuthorizationHeader);

        if (jsonBody is not null)
            req.Content = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/fhir+json");

        _logger.LogInformation("Forwarding terminology request to {Url}", fullUrl);

        using var resp = await client.SendAsync(req, cancellationToken);
        var body = await resp.Content.ReadAsStringAsync(cancellationToken);
        var media = resp.Content.Headers.ContentType?.MediaType ?? "application/fhir+json";
        return new RemoteForwardResult((int)resp.StatusCode, body, media);
    }

    private static string? TryGetQueryParam(string relativePathAndQuery, string name)
    {
        var qi = relativePathAndQuery.IndexOf('?', StringComparison.Ordinal);
        if (qi < 0)
            return null;

        var query = relativePathAndQuery[(qi + 1)..];
        foreach (var part in query.Split('&'))
        {
            var eq = part.IndexOf('=');
            if (eq <= 0)
                continue;
            var key = Uri.UnescapeDataString(part[..eq]);
            if (!string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
                continue;

            return Uri.UnescapeDataString(part[(eq + 1)..]);
        }

        return null;
    }
}

public sealed class NullRemoteTerminologyGateway : IRemoteTerminologyGateway
{
    public Task<RemoteForwardResult?> ForwardAsync(string relativePathAndQuery, HttpMethod method, string? jsonBody, CancellationToken cancellationToken = default)
        => Task.FromResult<RemoteForwardResult?>(null);
}
