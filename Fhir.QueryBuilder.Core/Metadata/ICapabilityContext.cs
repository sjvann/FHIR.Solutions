using Fhir.Resources.R5;

namespace Fhir.QueryBuilder.Metadata;

/// <summary>目前連線伺服器之 CapabilityStatement（由 <see cref="Services.FhirQueryService"/> 更新）。</summary>
public interface ICapabilityContext
{
    string? BaseUrl { get; }

    CapabilityStatement? Capability { get; }

    /// <summary>最近一次 <c>/metadata</c> 回應 JSON（用於列舉資源型別之後援解析）。</summary>
    string? LastCapabilityJson { get; }

    void SetConnection(string baseUrl, CapabilityStatement capability, string? capabilityJson = null);
}

public sealed class CapabilityContext : ICapabilityContext
{
    public string? BaseUrl { get; private set; }

    public CapabilityStatement? Capability { get; private set; }

    public string? LastCapabilityJson { get; private set; }

    public void SetConnection(string baseUrl, CapabilityStatement capability, string? capabilityJson = null)
    {
        BaseUrl = baseUrl.TrimEnd('/');
        Capability = capability;
        LastCapabilityJson = capabilityJson;
    }
}
