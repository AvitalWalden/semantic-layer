using System.Text.Json.Serialization;

namespace SemanticLayer.Application.Metadata;

/// <summary>
/// Strongly-typed representation of the external metadata file (metadata.json).
/// Property names are matched case-insensitively when deserializing.
/// </summary>
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
    /// <summary>Logical field name (mapping key within the entity).</summary>
    public string Name { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? Description { get; set; }

    /// <summary>SQL expression over the same table's columns.</summary>
    public string Expression { get; set; } = string.Empty;

    public string? DataType { get; set; }
    public bool? IsPii { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Domain.Enums.SensitivityLevel? SensitivityLevel { get; set; }

    public string? Unit { get; set; }
}
