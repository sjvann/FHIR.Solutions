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

public class TerminologyDbContext(DbContextOptions<TerminologyDbContext> options) : DbContext(options)
{
    public DbSet<TerminologyResourceEntity> TerminologyResources => Set<TerminologyResourceEntity>();
    public DbSet<BindingRegistryEntity> BindingRegistry => Set<BindingRegistryEntity>();
    public DbSet<UpstreamServerEntity> UpstreamServers => Set<UpstreamServerEntity>();

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
            e.HasIndex(x => new { x.ResourceType, x.LogicalId }).IsUnique();
        });
    }
}
