using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Infrastructure.Data;
using SemanticLayer.Infrastructure.Introspection;
using SemanticLayer.Infrastructure.Persistence;

namespace SemanticLayer.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        var dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.AddSingleton(dataSource);

        services.AddDbContext<SemanticDbContext>(options => options.UseNpgsql(dataSource));

        services.AddScoped<ISemanticRepository, EfSemanticRepository>();
        services.AddScoped<ISchemaIntrospector, PostgresSchemaIntrospector>();
        services.AddScoped<IDataQueryService, PostgresDataQueryService>();

        return services;
    }
}
