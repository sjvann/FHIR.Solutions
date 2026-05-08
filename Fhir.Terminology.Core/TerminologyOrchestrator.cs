using System.Net;
using System.Net.Http;
using Fhir.Resources.R5;
using Task = System.Threading.Tasks.Task;
using Fhir.TypeFramework.Bases;
using Fhir.TypeFramework.DataTypes;
using Fhir.TypeFramework.DataTypes.PrimitiveTypes;
using Microsoft.Extensions.Logging;

namespace Fhir.Terminology.Core;

public interface IRemoteTerminologyGateway
{
    /// <summary>若無適用上游術語伺服器則回傳 null。</summary>
    Task<RemoteForwardResult?> ForwardAsync(string relativePathAndQuery, HttpMethod method, string? jsonBody, CancellationToken cancellationToken = default);
}

public sealed record RemoteForwardResult(int StatusCode, string Body, string ContentType);

public sealed record FhirOperationResult(int HttpStatus, string ContentType, string JsonBody);

public sealed class TerminologyOrchestrator(
    ITerminologyRepository repository,
    IRemoteTerminologyGateway remote,
    ILogger<TerminologyOrchestrator> logger)
{
    private readonly ITerminologyRepository _repository = repository;
    private readonly IRemoteTerminologyGateway _remote = remote;
    private readonly ILogger<TerminologyOrchestrator> _logger = logger;

    public Task<FhirOperationResult> MetadataCapabilityAsync(CancellationToken ct)
    {
        var cs = BuildCapabilityStatement();
        return Task.FromResult(Ok(cs));
    }

    public async Task<FhirOperationResult> MetadataTerminologyAsync(CancellationToken ct)
    {
        var tc = await BuildTerminologyCapabilitiesAsync(ct);
        return Ok(tc);
    }

    public async Task<FhirOperationResult> SearchAsync(string resourceType, TerminologySearchParameters p, CancellationToken ct)
    {
        var rows = await _repository.SearchAsync(resourceType, p, ct);
        var bundle = new Bundle
        {
            Type = new FhirCode("searchset"),
            Total = new FhirUnsignedInt((uint)rows.Count),
            Entry = [],
        };

        foreach (var row in rows)
        {
            var resource = DeserializeKnown(row.ResourceType, row.RawJson);
            if (resource is null) continue;
            bundle.Entry ??= [];
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = new FhirUri($"urn:uuid:{row.RowId}"),
                Resource = resource,
                Search = new Bundle.EntryComponent.EntrySearchComponent { Mode = new FhirCode("match") },
            });
        }

        return Ok(bundle);
    }

    public async Task<FhirOperationResult?> ReadAsync(string resourceType, string id, CancellationToken ct)
    {
        var row = await _repository.GetAsync(resourceType, id, ct);
        if (row is null)
            return Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound($"{resourceType}/{id} not found"));

        return Ok(DeserializeKnown(resourceType, row.RawJson)!);
    }

    public async Task<FhirOperationResult> CreateAsync(string resourceType, string json, CancellationToken ct)
    {
        var resource = DeserializeKnown(resourceType, json);
        if (resource is null)
            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error("Invalid JSON resource"));

        if (resource is not DomainResource dr)
            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error("Expected DomainResource"));

        var rt = GetResourceType(dr);
        if (!string.Equals(rt, resourceType, StringComparison.Ordinal))
            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error($"resourceType mismatch: expected {resourceType}"));

        var stored = await _repository.CreateAsync(json, ct);
        var created = DeserializeKnown(resourceType, stored.RawJson);
        if (created is null)
            return Fail(HttpStatusCode.InternalServerError, OperationOutcomeFactory.Error("Round-trip serialize failed"));
        return new FhirOperationResult((int)HttpStatusCode.Created, "application/fhir+json", TerminologyJson.Serialize((Base)created));
    }

    public async Task<FhirOperationResult> UpdateAsync(string resourceType, string id, string json, CancellationToken ct)
    {
        var updated = await _repository.UpdateAsync(resourceType, id, json, ct);
        if (updated is null)
            return Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound($"{resourceType}/{id}"));

        var resource = DeserializeKnown(resourceType, updated.RawJson)!;
        return Ok(resource);
    }

    public async Task<FhirOperationResult> DeleteAsync(string resourceType, string id, CancellationToken ct)
    {
        var ok = await _repository.DeleteAsync(resourceType, id, ct);
        if (!ok)
            return Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound($"{resourceType}/{id}"));

        return new FhirOperationResult((int)HttpStatusCode.NoContent, "application/fhir+json", "");
    }

    public async Task<FhirOperationResult> ExpandValueSetAsync(
        string? valueSetId,
        string? urlParam,
        string? filter,
        int? offset,
        int? count,
        CancellationToken ct)
    {
        var query = BuildExpandQuery(valueSetId, urlParam, filter, offset, count);

        ValueSet? vs = null;
        if (!string.IsNullOrEmpty(valueSetId))
        {
            var row = await _repository.GetAsync("ValueSet", valueSetId, ct);
            if (row is null)
            {
                var rf = await _remote.ForwardAsync($"ValueSet/$expand{query}", HttpMethod.Get, null, ct);
                return rf is not null ? Forward(rf) : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("ValueSet not found"));
            }

            vs = DeserializeKnown(ValueSet.ResourceTypeValue, row.RawJson) as ValueSet;
        }
        else if (!string.IsNullOrEmpty(urlParam))
        {
            var row = await _repository.FindByUrlAsync("ValueSet", urlParam, version: null, ct);
            if (row is null)
            {
                var rf = await _remote.ForwardAsync($"ValueSet/$expand{query}", HttpMethod.Get, null, ct);
                return rf is not null
                    ? Forward(rf)
                    : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("ValueSet url not found locally and no upstream"));
            }

            vs = DeserializeKnown(ValueSet.ResourceTypeValue, row.RawJson) as ValueSet;
        }
        else
            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error("Provide url or ValueSet instance path"));

        if (vs is null)
            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error("Cannot parse ValueSet"));

        try
        {
            var expanded = await ExpandValueSetLocalAsync(vs, filter, offset, count, ct);
            return Ok(expanded);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Local expand failed, trying upstream");
            var forwarded = await _remote.ForwardAsync($"ValueSet/$expand{query}", HttpMethod.Get, null, ct);
            if (forwarded is not null)
                return Forward(forwarded);

            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error(ex.Message));
        }
    }

    private static string BuildExpandQuery(string? valueSetId, string? urlParam, string? filter, int? offset, int? count)
    {
        var q = new List<string>();
        if (!string.IsNullOrEmpty(urlParam))
            q.Add("url=" + Uri.EscapeDataString(urlParam));
        if (!string.IsNullOrEmpty(filter))
            q.Add("filter=" + Uri.EscapeDataString(filter));
        if (offset is not null)
            q.Add("offset=" + offset.Value);
        if (count is not null)
            q.Add("count=" + count.Value);
        return q.Count == 0 ? "" : "?" + string.Join("&", q);
    }

    private async Task<ValueSet> ExpandValueSetLocalAsync(ValueSet vs, string? filter, int? offset, int? count, CancellationToken ct)
    {
        if (vs.Compose is null)
        {
            if (vs.Expansion?.Contains is { Count: > 0 })
                return vs;

            throw new InvalidOperationException("ValueSet has no compose and no expansion");
        }

        var caseInsensitive = StringComparison.Ordinal;
        var allContains = new List<ValueSet.ExpansionComponent.ExpansionContainsComponent>();

        foreach (var inc in vs.Compose.Include ?? [])
        {
            var sys = FhirPrimitive.Uri(inc.System);
            if (sys is null)
                continue;

            var csRow = await _repository.FindByUrlAsync("CodeSystem", sys, FhirPrimitive.String(inc.Version), ct)
                         ?? await _repository.FindByUrlAsync("CodeSystem", sys, null, ct);

            if (csRow is null)
                throw new InvalidOperationException($"CodeSystem not found for system {sys}");

            var csJson = csRow.RawJson;
            var codes = SelectIncludedCodes(inc, csJson, caseInsensitive);
            foreach (var (code, display) in codes)
            {
                if (!string.IsNullOrEmpty(filter) &&
                    (display is null || display.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) &&
                    code.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                allContains.Add(new ValueSet.ExpansionComponent.ExpansionContainsComponent
                {
                    System = new FhirUri(sys),
                    Code = new FhirCode(code),
                    Display = display is not null ? new FhirString(display) : null,
                });
            }
        }

        var total = allContains.Count;
        var off = offset ?? 0;
        var take = count ?? total;
        if (off < 0) off = 0;
        var page = allContains.Skip(off).Take(take).ToList();

        vs.Expansion = new ValueSet.ExpansionComponent
        {
            Total = new FhirInteger(total),
            Offset = offset is not null ? new FhirInteger(off) : null,
            Contains = page,
        };

        return vs;
    }

    private static IEnumerable<(string code, string? display)> SelectIncludedCodes(
        ValueSet.ComposeComponent.ComposeIncludeComponent inc,
        string codeSystemJson,
        StringComparison cmp)
    {
        var all = CodeSystemCodeIndex.ListCodes(codeSystemJson);

        if (inc.Concept is { Count: > 0 })
        {
            foreach (var cc in inc.Concept)
            {
                var code = FhirPrimitive.Code(cc.Code);
                if (code is null) continue;
                var match = all.FirstOrDefault(x => string.Equals(x.Code, code, cmp));
                yield return (code, match.Display ?? FhirPrimitive.String(cc.Display));
            }

            yield break;
        }

        if (inc.Filter is { Count: > 0 })
        {
            foreach (var f in inc.Filter)
            {
                var prop = FhirPrimitive.Code(f.Property);
                var op = FhirPrimitive.Code(f.Op);
                var val = FhirPrimitive.String(f.Value);
                if (prop == "concept" && op == "=" && val is not null)
                {
                    foreach (var (code, display) in all)
                    {
                        if (code == val)
                            yield return (code, display);
                    }
                }
            }

            yield break;
        }

        foreach (var pair in all)
            yield return pair;
    }

    public async Task<FhirOperationResult> ValidateCodeValueSetAsync(
        string? valueSetId,
        string? url,
        string? code,
        string? system,
        string? display,
        CancellationToken ct)
    {
        var q = BuildValidateVsQuery(url, code, system, display);

        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(system))
            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error("code and system are required"));

        ValueSet? vs = null;
        if (!string.IsNullOrEmpty(valueSetId))
        {
            var row = await _repository.GetAsync("ValueSet", valueSetId, ct);
            vs = row is null ? null : DeserializeKnown(ValueSet.ResourceTypeValue, row.RawJson) as ValueSet;
        }
        else if (!string.IsNullOrEmpty(url))
        {
            var row = await _repository.FindByUrlAsync("ValueSet", url, null, ct);
            vs = row is null ? null : DeserializeKnown(ValueSet.ResourceTypeValue, row.RawJson) as ValueSet;
        }

        if (vs is not null)
        {
            var expanded = await ExpandValueSetLocalAsync(vs, filter: null, offset: null, count: null, ct);
            var ok = expanded.Expansion?.Contains?.Any(c =>
                FhirPrimitive.Code(c.Code) == code &&
                (FhirPrimitive.Uri(c.System) == system)) == true;

            var p = new Parameters { Parameter = [] };
            p.Parameter!.Add(new Parameters.ParameterComponent { Name = new FhirString("result"), ValueBoolean = new FhirBoolean(ok) });
            if (ok)
                p.Parameter.Add(new Parameters.ParameterComponent { Name = new FhirString("message"), ValueString = new FhirString("Code valid") });
            else
                p.Parameter.Add(new Parameters.ParameterComponent { Name = new FhirString("message"), ValueString = new FhirString("Code not in value set expansion") });

            return Ok(p);
        }

        var forwarded = await _remote.ForwardAsync($"ValueSet/$validate-code{q}", HttpMethod.Get, null, ct);
        return forwarded is not null
            ? Forward(forwarded)
            : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("ValueSet not found"));
    }

    /// <summary>依 FHIR Binding 登錄（StructureDefinition URL + 元素 path）解析 ValueSet canonical 後執行與 ValueSet/$validate-code 相同語意。</summary>
    public async Task<FhirOperationResult> ValidateCodeWithBindingContextAsync(
        string? profileUrl,
        string? elementPath,
        string? code,
        string? system,
        string? display,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(profileUrl) || string.IsNullOrEmpty(elementPath))
            return Fail(HttpStatusCode.BadRequest, OperationOutcomeFactory.Error("profile and element path are required (query: profile, path)"));

        var bindings = await _repository.ListBindingsAsync(ct);
        var match = bindings.FirstOrDefault(b =>
            string.Equals(b.StructureDefinitionUrl, profileUrl, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(b.ElementPath, elementPath, StringComparison.OrdinalIgnoreCase));

        if (match is null)
            return Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("No binding registry entry for profile/path"));

        return await ValidateCodeValueSetAsync(null, match.ValueSetCanonical, code, system, display, ct);
    }

    private static string BuildValidateVsQuery(string? url, string? code, string? system, string? display)
    {
        var q = new List<string>();
        if (!string.IsNullOrEmpty(url))
            q.Add("url=" + Uri.EscapeDataString(url));
        if (!string.IsNullOrEmpty(code))
            q.Add("code=" + Uri.EscapeDataString(code));
        if (!string.IsNullOrEmpty(system))
            q.Add("system=" + Uri.EscapeDataString(system));
        if (!string.IsNullOrEmpty(display))
            q.Add("display=" + Uri.EscapeDataString(display));
        return q.Count == 0 ? "" : "?" + string.Join("&", q);
    }

    public async Task<FhirOperationResult> ValidateCodeCodeSystemAsync(string? codeSystemId, string? url, string code, string? system, string? display, CancellationToken ct)
    {
        var qs = new List<string>();
        if (!string.IsNullOrEmpty(url))
            qs.Add("url=" + Uri.EscapeDataString(url));
        qs.Add("code=" + Uri.EscapeDataString(code));
        if (!string.IsNullOrEmpty(system))
            qs.Add("system=" + Uri.EscapeDataString(system));
        var q = qs.Count == 0 ? "" : "?" + string.Join("&", qs);

        StoredResourceRow? row = null;
        if (!string.IsNullOrEmpty(codeSystemId))
            row = await _repository.GetAsync("CodeSystem", codeSystemId, ct);
        else if (!string.IsNullOrEmpty(url))
            row = await _repository.FindByUrlAsync("CodeSystem", url, null, ct);

        if (row is not null)
        {
            var codes = CodeSystemCodeIndex.ListCodes(row.RawJson);
            var ok = codes.Any(c => c.Code == code);
            var p = new Parameters { Parameter = [] };
            p.Parameter!.Add(new Parameters.ParameterComponent { Name = new FhirString("result"), ValueBoolean = new FhirBoolean(ok) });
            p.Parameter.Add(new Parameters.ParameterComponent { Name = new FhirString("message"), ValueString = new FhirString(ok ? "Valid" : "Unknown code") });
            return Ok(p);
        }

        var forwarded = await _remote.ForwardAsync($"CodeSystem/$validate-code{q}", HttpMethod.Get, null, ct);
        return forwarded is not null
            ? Forward(forwarded)
            : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("CodeSystem not found"));
    }

    public async Task<FhirOperationResult> LookupAsync(string? codeSystemId, string? system, string code, string? version, CancellationToken ct)
    {
        var q = new List<string>();
        if (!string.IsNullOrEmpty(system))
            q.Add("system=" + Uri.EscapeDataString(system));
        q.Add("code=" + Uri.EscapeDataString(code));
        if (!string.IsNullOrEmpty(version))
            q.Add("version=" + Uri.EscapeDataString(version));
        var query = "?" + string.Join("&", q);

        StoredResourceRow? row = null;
        if (!string.IsNullOrEmpty(codeSystemId))
            row = await _repository.GetAsync("CodeSystem", codeSystemId, ct);
        else if (!string.IsNullOrEmpty(system))
            row = await _repository.FindByUrlAsync("CodeSystem", system, version, ct)
                  ?? await _repository.FindByUrlAsync("CodeSystem", system, null, ct);

        if (row is not null)
        {
            foreach (var (c, d) in CodeSystemCodeIndex.ListCodes(row.RawJson))
            {
                if (c == code)
                {
                    var p = new Parameters
                    {
                        Parameter =
                        [
                            new Parameters.ParameterComponent { Name = new FhirString("display"), ValueString = new FhirString(d ?? code) },
                        ],
                    };
                    return Ok(p);
                }
            }

            var forwardedAfterMiss = await _remote.ForwardAsync($"CodeSystem/$lookup{query}", HttpMethod.Get, null, ct);
            return forwardedAfterMiss is not null
                ? Forward(forwardedAfterMiss)
                : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("Code not found in CodeSystem"));
        }

        var forwarded = await _remote.ForwardAsync($"CodeSystem/$lookup{query}", HttpMethod.Get, null, ct);
        return forwarded is not null
            ? Forward(forwarded)
            : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("CodeSystem not found"));
    }

    public async Task<FhirOperationResult> SubsumesAsync(string? codeSystemId, string system, string codeA, string codeB, string? version, CancellationToken ct)
    {
        var q = new List<string>
        {
            "system=" + Uri.EscapeDataString(system),
            "codeA=" + Uri.EscapeDataString(codeA),
            "codeB=" + Uri.EscapeDataString(codeB),
        };
        if (!string.IsNullOrEmpty(version))
            q.Add("version=" + Uri.EscapeDataString(version));
        var query = "?" + string.Join("&", q);

        StoredResourceRow? row = null;
        if (!string.IsNullOrEmpty(codeSystemId))
            row = await _repository.GetAsync("CodeSystem", codeSystemId, ct);
        else
            row = await _repository.FindByUrlAsync("CodeSystem", system, version, ct)
                  ?? await _repository.FindByUrlAsync("CodeSystem", system, null, ct);

        if (row is not null)
        {
            var outcome = CodeSystemCodeIndex.ComputeSubsumes(row.RawJson, codeA, codeB);
            var p = new Parameters
            {
                Parameter =
                [
                    new Parameters.ParameterComponent { Name = new FhirString("outcome"), ValueCode = new FhirCode(outcome) },
                ],
            };
            return Ok(p);
        }

        var forwarded = await _remote.ForwardAsync($"CodeSystem/$subsumes{query}", HttpMethod.Get, null, ct);
        return forwarded is not null
            ? Forward(forwarded)
            : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("CodeSystem not found"));
    }

    public async Task<FhirOperationResult> TranslateAsync(string? conceptMapId, string? url, string sourceCode, string sourceSystem, string? targetSystem, CancellationToken ct)
    {
        var q = new List<string> { "code=" + Uri.EscapeDataString(sourceCode), "system=" + Uri.EscapeDataString(sourceSystem) };
        if (!string.IsNullOrEmpty(targetSystem))
            q.Add("targetsystem=" + Uri.EscapeDataString(targetSystem));
        if (!string.IsNullOrEmpty(url))
            q.Add("url=" + Uri.EscapeDataString(url));
        var query = "?" + string.Join("&", q);

        ConceptMap? cm = null;
        if (!string.IsNullOrEmpty(conceptMapId))
        {
            var row = await _repository.GetAsync("ConceptMap", conceptMapId, ct);
            cm = row is null ? null : DeserializeKnown(ConceptMap.ResourceTypeValue, row.RawJson) as ConceptMap;
        }
        else if (!string.IsNullOrEmpty(url))
        {
            var row = await _repository.FindByUrlAsync("ConceptMap", url, null, ct);
            cm = row is null ? null : DeserializeKnown(ConceptMap.ResourceTypeValue, row.RawJson) as ConceptMap;
        }

        if (cm is not null)
        {
            foreach (var g in cm.Group ?? [])
            {
                var src = g.Source?.StringValue;
                var tgt = g.Target?.StringValue;
                if (src is not null && !sourceSystem.StartsWith(src, StringComparison.Ordinal) && src != sourceSystem)
                    continue;

                foreach (var el in g.Element ?? [])
                {
                    if (FhirPrimitive.Code(el.Code) != sourceCode)
                        continue;

                    foreach (var t in el.Target ?? [])
                    {
                        if (targetSystem is not null && tgt is not null && !targetSystem.StartsWith(tgt, StringComparison.Ordinal))
                            continue;

                        var p = new Parameters
                        {
                            Parameter =
                            [
                                new Parameters.ParameterComponent { Name = new FhirString("match"), ValueBoolean = new FhirBoolean(true) },
                                new Parameters.ParameterComponent
                                {
                                    Name = new FhirString("concept"),
                                    ValueCoding = new Coding
                                    {
                                        System = string.IsNullOrEmpty(tgt) ? new FhirUri(sourceSystem) : new FhirUri(tgt),
                                        Code = FhirPrimitive.Code(t.Code),
                                        Display = FhirPrimitive.String(t.Display),
                                    },
                                },
                            ],
                        };
                        return Ok(p);
                    }
                }
            }
        }

        var forwarded = await _remote.ForwardAsync($"ConceptMap/$translate{query}", HttpMethod.Get, null, ct);
        return forwarded is not null
            ? Forward(forwarded)
            : Fail(HttpStatusCode.NotFound, cm is not null
                ? OperationOutcomeFactory.NotFound("No translation")
                : OperationOutcomeFactory.NotFound("ConceptMap not found"));
    }

    public async Task<FhirOperationResult> ListUpstreamsBundleAsync(CancellationToken ct)
    {
        var rows = await _repository.ListUpstreamsAsync(ct);
        var bundle = new Bundle
        {
            Type = new FhirCode("collection"),
            Entry = [],
        };

        foreach (var u in rows)
        {
            var parameters = new Parameters
            {
                Parameter =
                [
                    new() { Name = new FhirString("id"), ValueString = new FhirString(u.Id.ToString("D")) },
                    new() { Name = new FhirString("systemUriPrefix"), ValueString = new FhirString(u.SystemUriPrefix) },
                    new() { Name = new FhirString("baseUrl"), ValueString = new FhirString(u.BaseUrl) },
                    new() { Name = new FhirString("timeoutSeconds"), ValueString = new FhirString(u.TimeoutSeconds.ToString()) },
                ],
            };
            bundle.Entry!.Add(new Bundle.EntryComponent { Resource = parameters });
        }

        return Ok(bundle);
    }

    public async Task<FhirOperationResult> AddUpstreamRowAsync(UpstreamServerRow row, CancellationToken ct)
    {
        var created = await _repository.AddUpstreamAsync(row, ct);
        var p = new Parameters
        {
            Parameter =
            [
                new() { Name = new FhirString("id"), ValueString = new FhirString(created.Id.ToString("D")) },
            ],
        };
        return new FhirOperationResult((int)HttpStatusCode.Created, "application/fhir+json", TerminologyJson.Serialize(p));
    }

    public async Task<FhirOperationResult> DeleteUpstreamRowAsync(Guid id, CancellationToken ct)
    {
        var ok = await _repository.DeleteUpstreamAsync(id, ct);
        return ok
            ? new FhirOperationResult((int)HttpStatusCode.NoContent, "application/fhir+json", "")
            : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("Upstream not found"));
    }

    public async Task<FhirOperationResult> DeleteBindingRowAsync(Guid id, CancellationToken ct)
    {
        var ok = await _repository.DeleteBindingAsync(id, ct);
        return ok
            ? new FhirOperationResult((int)HttpStatusCode.NoContent, "application/fhir+json", "")
            : Fail(HttpStatusCode.NotFound, OperationOutcomeFactory.NotFound("Binding not found"));
    }

    public async Task<FhirOperationResult> AddBindingRowAsync(BindingRegistryRow row, CancellationToken ct)
    {
        var created = await _repository.AddBindingAsync(row, ct);
        var p = new Parameters
        {
            Parameter =
            [
                new() { Name = new FhirString("id"), ValueString = new FhirString(created.Id.ToString("D")) },
            ],
        };
        return new FhirOperationResult((int)HttpStatusCode.Created, "application/fhir+json", TerminologyJson.Serialize(p));
    }

    public async Task<FhirOperationResult> ListBindingsAsync(CancellationToken ct)
    {
        var rows = await _repository.ListBindingsAsync(ct);
        var bundle = new Bundle
        {
            Type = new FhirCode("collection"),
            Entry = [],
        };

        foreach (var b in rows)
        {
            var parameters = new Parameters
            {
                Parameter =
                [
                    new() { Name = new FhirString("id"), ValueString = new FhirString(b.Id.ToString("D")) },
                    new() { Name = new FhirString("structureDefinition"), ValueUri = string.IsNullOrEmpty(b.StructureDefinitionUrl) ? null : new FhirUri(b.StructureDefinitionUrl) },
                    new() { Name = new FhirString("path"), ValueString = string.IsNullOrEmpty(b.ElementPath) ? null : new FhirString(b.ElementPath) },
                    new() { Name = new FhirString("valueSet"), ValueUri = new FhirUri(b.ValueSetCanonical) },
                    new() { Name = new FhirString("strength"), ValueCode = new FhirCode(b.Strength) },
                ],
            };
            bundle.Entry!.Add(new Bundle.EntryComponent { Resource = parameters });
        }

        return Ok(bundle);
    }

    private async Task<TerminologyCapabilities> BuildTerminologyCapabilitiesAsync(CancellationToken ct)
    {
        var systems = await _repository.SearchAsync("CodeSystem", new TerminologySearchParameters(), ct);
        var tc = new TerminologyCapabilities
        {
            Url = new FhirUri("http://localhost/fhir/terminology-capabilities"),
            Name = new FhirString("TerminologyServer"),
            Title = new FhirString("FHIR Terminology"),
            Status = new FhirCode("active"),
            Date = new FhirDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd")),
            Kind = new FhirCode("instance"),
            CodeSystem = [],
        };

        foreach (var row in systems)
        {
            var cs = DeserializeKnown(CodeSystem.ResourceTypeValue, row.RawJson) as CodeSystem;
            if (cs is null) continue;
            var urlStr = FhirPrimitive.Uri(cs.Url);
            if (string.IsNullOrEmpty(urlStr))
                continue;

            List<TerminologyCapabilities.CodeSystemComponent.CodeSystemVersionComponent>? versions = null;
            var ver = FhirPrimitive.String(cs.Version);
            if (!string.IsNullOrEmpty(ver))
            {
                versions =
                [
                    new TerminologyCapabilities.CodeSystemComponent.CodeSystemVersionComponent { Code = new FhirString(ver) },
                ];
            }

            tc.CodeSystem!.Add(new TerminologyCapabilities.CodeSystemComponent
            {
                Uri = new FhirCanonical(urlStr),
                Version = versions,
                Content = new FhirCode("complete"),
            });
        }

        return tc;
    }

    private static CapabilityStatement BuildCapabilityStatement()
    {
        return new CapabilityStatement
        {
            Url = new FhirUri("http://localhost/fhir/metadata"),
            Name = new FhirString("TerminologyService"),
            Title = new FhirString("FHIR Terminology Service"),
            Status = new FhirCode("active"),
            Date = new FhirDateTime(DateTime.UtcNow.ToString("yyyy-MM-dd")),
            Description = new FhirMarkdown("Terminology server per FHIR R5"),
            Kind = new FhirCode("instance"),
            FhirVersion = new FhirCode("5.0.0"),
            Format = [new FhirCode("json")],
            Rest =
            [
                new CapabilityStatement.RestComponent
                {
                    Mode = new FhirCode("server"),
                    Resource =
                    [
                        CapabilityResources.CodeSystem(),
                        CapabilityResources.ValueSet(),
                        CapabilityResources.ConceptMap(),
                    ],
                },
            ],
        };
    }

    private static DomainResource? DeserializeKnown(string resourceType, string json) =>
        resourceType switch
        {
            CodeSystem.ResourceTypeValue => TerminologyJson.Deserialize<CodeSystem>(json),
            ValueSet.ResourceTypeValue => TerminologyJson.Deserialize<ValueSet>(json),
            ConceptMap.ResourceTypeValue => TerminologyJson.Deserialize<ConceptMap>(json),
            _ => TerminologyJson.DeserializeResource(json) as DomainResource,
        };

    private static string GetResourceType(DomainResource r) => r switch
    {
        CodeSystem => CodeSystem.ResourceTypeValue,
        ValueSet => ValueSet.ResourceTypeValue,
        ConceptMap => ConceptMap.ResourceTypeValue,
        _ => "",
    };

    private static FhirOperationResult Ok(Resource r) =>
        new((int)HttpStatusCode.OK, "application/fhir+json", TerminologyJson.Serialize((Base)r));

    private static FhirOperationResult Fail(HttpStatusCode status, OperationOutcome oo) =>
        new((int)status, "application/fhir+json", TerminologyJson.Serialize((Base)oo));

    private static FhirOperationResult Forward(RemoteForwardResult f)
    {
        var ct = string.IsNullOrEmpty(f.ContentType) ? "application/fhir+json" : f.ContentType;
        return new FhirOperationResult(f.StatusCode, ct, f.Body);
    }
}
