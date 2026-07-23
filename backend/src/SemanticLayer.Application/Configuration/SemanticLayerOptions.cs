namespace SemanticLayer.Application.Configuration;

public class SemanticLayerOptions
{
    public const string SectionName = "SemanticLayer";

    public string SourceSchema { get; set; } = "hr";

    public string DataSourceName { get; set; } = "HR Database";

    public string? SeedMetadataPath { get; set; }
}
