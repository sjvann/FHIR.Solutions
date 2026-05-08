using System.ComponentModel.DataAnnotations;

namespace Fhir.Terminology.Core;

/// <summary>術語 App 之 SQLite 與主機相關組態（搭配 <see cref="Microsoft.Extensions.Options"/>）。</summary>
public sealed class TerminologyServerOptions
{
    public const string SectionName = "Terminology";

    /// <summary>SQLite 連線字串（通常來自 <c>ConnectionStrings:Terminology</c>）。</summary>
    [Required]
    public string ConnectionString { get; set; } = "Data Source=terminology.db";
}
