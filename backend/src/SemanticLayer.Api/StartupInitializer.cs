using Microsoft.EntityFrameworkCore;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Configuration;
using SemanticLayer.Infrastructure.Persistence;

namespace SemanticLayer.Api;

/// <summary>
/// Prepares the database on startup: applies EF Core migrations (creating the
/// "semantic" schema), ensures a default data source exists, and optionally runs
/// an initial schema sync so the app is usable immediately after boot.
/// </summary>
public static class StartupInitializer
{
    public static async Task RunAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("StartupInitializer");
        var config = sp.GetRequiredService<IConfiguration>();

        var db = sp.GetRequiredService<SemanticDbContext>();

        await WaitForDatabaseAsync(db, logger);

        logger.LogInformation("Applying EF Core migrations...");
        await db.Database.MigrateAsync();

        var repo = sp.GetRequiredService<ISemanticRepository>();
        var options = config.GetSection(SemanticLayerOptions.SectionName).Get<SemanticLayerOptions>()
                      ?? new SemanticLayerOptions();

        await repo.GetOrCreateDefaultDataSourceAsync(options.DataSourceName, options.SourceSchema);

        var autoSync = config.GetValue("SemanticLayer:AutoSyncSchemaOnStartup", true);
        var entities = await repo.GetEntitiesAsync(includeFields: false, onlyVisible: false);

        if (autoSync && entities.Count == 0)
        {
            logger.LogInformation("Semantic layer is empty; running initial schema sync...");
            var syncService = sp.GetRequiredService<ISyncService>();
            var result = await syncService.SyncSchemaAsync();
            logger.LogInformation("Initial sync complete: {Summary}", result.Summary);
        }
    }

    private static async Task WaitForDatabaseAsync(SemanticDbContext db, ILogger logger)
    {
        const int maxAttempts = 15;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                if (await db.Database.CanConnectAsync())
                    return;
            }
            catch (Exception ex)
            {
                logger.LogWarning("Database not ready (attempt {Attempt}/{Max}): {Message}", attempt, maxAttempts, ex.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        logger.LogWarning("Proceeding without confirmed database connectivity after {Max} attempts.", maxAttempts);
    }
}
