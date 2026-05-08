using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fhir.Terminology.Infrastructure;

public class TerminologyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TerminologyDbContext>
{
    public TerminologyDbContext CreateDbContext(string[] args)
    {
        var o = new DbContextOptionsBuilder<TerminologyDbContext>();
        o.UseSqlite("Data Source=terminology.design.db");
        return new TerminologyDbContext(o.Options);
    }
}
