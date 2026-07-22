using Microsoft.EntityFrameworkCore;
using SemanticLayer.Domain.Entities;

namespace SemanticLayer.Infrastructure.Persistence;

/// <summary>
/// EF Core context for the semantic metadata store. All tables live in the
/// dedicated "semantic" schema, separate from the physical source schema ("hr").
/// </summary>
public class SemanticDbContext : DbContext
{
    public const string Schema = "semantic";

    public SemanticDbContext(DbContextOptions<SemanticDbContext> options) : base(options) { }

    public DbSet<DataSource> DataSources => Set<DataSource>();
    public DbSet<SemanticEntity> Entities => Set<SemanticEntity>();
    public DbSet<SemanticField> Fields => Set<SemanticField>();
    public DbSet<SyncRun> SyncRuns => Set<SyncRun>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema(Schema);

        b.Entity<DataSource>(e =>
        {
            e.ToTable("data_sources");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.SourceSchema).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.HasIndex(x => x.Name).IsUnique();
        });

        b.Entity<SemanticEntity>(e =>
        {
            e.ToTable("semantic_entities");
            e.HasKey(x => x.Id);
            e.Property(x => x.PhysicalTableName).HasMaxLength(200).IsRequired();
            e.Property(x => x.BusinessName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.PrimaryKeyColumn).HasMaxLength(200);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

            e.HasOne(x => x.DataSource)
                .WithMany(d => d.Entities)
                .HasForeignKey(x => x.DataSourceId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.DataSourceId, x.PhysicalTableName }).IsUnique();
        });

        b.Entity<SemanticField>(e =>
        {
            e.ToTable("semantic_fields");
            e.HasKey(x => x.Id);
            e.Property(x => x.PhysicalColumnName).HasMaxLength(200).IsRequired();
            e.Property(x => x.BusinessName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
            e.Property(x => x.PhysicalDataType).HasMaxLength(100);
            e.Property(x => x.Unit).HasMaxLength(50);
            e.Property(x => x.DisplayFormat).HasMaxLength(100);
            e.Property(x => x.DerivedExpression).HasMaxLength(2000);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.SensitivityLevel).HasConversion<string>().HasMaxLength(20);

            e.HasOne(x => x.Entity)
                .WithMany(en => en.Fields)
                .HasForeignKey(x => x.EntityId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.EntityId, x.PhysicalColumnName }).IsUnique();
        });

        b.Entity<SyncRun>(e =>
        {
            e.ToTable("sync_runs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Summary).HasMaxLength(2000);
        });
    }
}
