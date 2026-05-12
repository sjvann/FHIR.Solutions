using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Fhir.Terminology.Infrastructure;

public class TerminologyResourceEntity
{
    [Key] public Guid Id { get; set; }

    [MaxLength(64)]
    public string ResourceType { get; set; } = "";

    [MaxLength(256)]
    public string LogicalId { get; set; } = "";

    public string RawJson { get; set; } = "";

    [MaxLength(1024)]
    public string? Url { get; set; }

    [MaxLength(256)]
    public string? Version { get; set; }

    [MaxLength(256)]
    public string? Name { get; set; }

    [MaxLength(512)]
    public string? Title { get; set; }

    [MaxLength(32)]
    public string? Status { get; set; }

    /// <summary>資源所依循之 FHIR 規格版本（例：<c>5.0.0</c>、<c>4.0.1</c>），與資源本身的 business <see cref="Version"/> 不同。</summary>
    [MaxLength(32)]
    public string FhirSpecVersion { get; set; } = "5.0.0";
}

public class BindingRegistryEntity
{
    [Key] public Guid Id { get; set; }

    [MaxLength(1024)]
    public string? StructureDefinitionUrl { get; set; }

    [MaxLength(512)]
    public string? ElementPath { get; set; }

    [MaxLength(1024)]
    public string ValueSetCanonical { get; set; } = "";

    [MaxLength(32)]
    public string Strength { get; set; } = "";
}

public class UpstreamServerEntity
{
    [Key] public Guid Id { get; set; }

    [MaxLength(512)]
    public string SystemUriPrefix { get; set; } = "";

    [MaxLength(2048)]
    public string BaseUrl { get; set; } = "";

    [MaxLength(4096)]
    public string? AuthorizationHeader { get; set; }

    public int TimeoutSeconds { get; set; } = 60;
}

/// <summary>記錄自 FHIR NPM 套件（例如 hl7.terminology）匯入之版本與校驗資訊。</summary>
public class TerminologyPackageImportEntity
{
    [Key] public Guid Id { get; set; }

    [MaxLength(128)]
    public string PackageId { get; set; } = "";

    [MaxLength(64)]
    public string PackageVersion { get; set; } = "";

    [MaxLength(32)]
    public string? FhirVersion { get; set; }

    public DateTimeOffset ImportedAtUtc { get; set; }

    [MaxLength(2048)]
    public string? SourceUri { get; set; }

    [MaxLength(128)]
    public string? Sha256Hex { get; set; }

    public int ResourceCount { get; set; }

    [MaxLength(4000)]
    public string? Notes { get; set; }
}

public class TerminologyDbContext(DbContextOptions<TerminologyDbContext> options) : DbContext(options)
{
    public DbSet<TerminologyResourceEntity> TerminologyResources => Set<TerminologyResourceEntity>();
    public DbSet<BindingRegistryEntity> BindingRegistry => Set<BindingRegistryEntity>();
    public DbSet<UpstreamServerEntity> UpstreamServers => Set<UpstreamServerEntity>();
    public DbSet<TerminologyPackageImportEntity> TerminologyPackageImports => Set<TerminologyPackageImportEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TerminologyResourceEntity>(e =>
        {
            e.HasIndex(x => x.ResourceType);
            e.HasIndex(x => x.LogicalId);
            e.HasIndex(x => x.Url);
            e.HasIndex(x => x.Version);
            e.HasIndex(x => x.Name);
            e.HasIndex(x => x.Title);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.FhirSpecVersion);
            e.HasIndex(x => new { x.ResourceType, x.LogicalId, x.FhirSpecVersion }).IsUnique();
        });

        modelBuilder.Entity<TerminologyPackageImportEntity>(e =>
        {
            e.HasIndex(x => x.ImportedAtUtc);
            e.HasIndex(x => new { x.PackageId, x.PackageVersion });
        });
    }
}
