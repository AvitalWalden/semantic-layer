using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SemanticLayer.Infrastructure.Persistence;

/// <summary>
/// Used by the EF Core tools (dotnet ef) at design time to create the context
/// without booting the whole application. The connection string here is only
/// used for tooling; it is not needed for generating migrations.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SemanticDbContext>
{
    public SemanticDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=semantic_demo;Username=semantic;Password=semantic";

        var options = new DbContextOptionsBuilder<SemanticDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new SemanticDbContext(options);
    }
}
