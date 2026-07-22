namespace SemanticLayer.Domain.Entities;

public class DataSource
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SourceSchema { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SemanticEntity> Entities { get; set; } = new List<SemanticEntity>();
}
