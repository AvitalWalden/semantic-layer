namespace SemanticLayer.Application.Configuration;

/// <summary>Configuration bound from the "SemanticLayer" section of appsettings.</summary>
public class SemanticLayerOptions
{
    public const string SectionName = "SemanticLayer";

    /// <summary>Physical schema in the source database to introspect.</summary>
    public string SourceSchema { get; set; } = "hr";

    /// <summary>Display name of the default data source.</summary>
    public string DataSourceName { get; set; } = "HR Database";

    /// <summary>Path to the metadata file auto-merged on startup (optional).</summary>
    public string? SeedMetadataPath { get; set; }
}
