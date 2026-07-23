using System.Text.Json.Serialization;

namespace SemanticLayer.Application.Metadata;

public class MetadataFile
{
    public string? Version { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, TableMetadata> Tables { get; set; } = new();
}

public class TableMetadata
{
    public string? BusinessName { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, ColumnMetadata> Columns { get; set; } = new();
    public List<DerivedFieldMetadata> DerivedFields { get; set; } = new();
}

public class ColumnMetadata
{
    public string? BusinessName { get; set; }
    public string? Description { get; set; }
    public bool? IsVisible { get; set; }
    public bool? IsPii { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Domain.Enums.SensitivityLevel? SensitivityLevel { get; set; }

    public string? Unit { get; set; }
    public string? DisplayFormat { get; set; }
}

public class DerivedFieldMetadata
{
    public string Name { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? Description { get; set; }

    public string Expression { get; set; } = string.Empty;

    public string? DataType { get; set; }
    public bool? IsPii { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Domain.Enums.SensitivityLevel? SensitivityLevel { get; set; }

    public string? Unit { get; set; }
}
