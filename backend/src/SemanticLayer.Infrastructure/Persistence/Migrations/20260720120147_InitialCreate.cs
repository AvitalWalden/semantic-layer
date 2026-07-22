using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SemanticLayer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "semantic");

            migrationBuilder.CreateTable(
                name: "data_sources",
                schema: "semantic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceSchema = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_data_sources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sync_runs",
                schema: "semantic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EntitiesAdded = table.Column<int>(type: "integer", nullable: false),
                    EntitiesRemoved = table.Column<int>(type: "integer", nullable: false),
                    FieldsAdded = table.Column<int>(type: "integer", nullable: false),
                    FieldsUpdated = table.Column<int>(type: "integer", nullable: false),
                    FieldsRemoved = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync_runs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "semantic_entities",
                schema: "semantic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataSourceId = table.Column<int>(type: "integer", nullable: false),
                    PhysicalTableName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BusinessName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PrimaryKeyColumn = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsUserModified = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semantic_entities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_semantic_entities_data_sources_DataSourceId",
                        column: x => x.DataSourceId,
                        principalSchema: "semantic",
                        principalTable: "data_sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "semantic_fields",
                schema: "semantic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    PhysicalColumnName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BusinessName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhysicalDataType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsVisible = table.Column<bool>(type: "boolean", nullable: false),
                    IsPii = table.Column<bool>(type: "boolean", nullable: false),
                    SensitivityLevel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DisplayFormat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsDerived = table.Column<bool>(type: "boolean", nullable: false),
                    DerivedExpression = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    IsUserModified = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semantic_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_semantic_fields_semantic_entities_EntityId",
                        column: x => x.EntityId,
                        principalSchema: "semantic",
                        principalTable: "semantic_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_data_sources_Name",
                schema: "semantic",
                table: "data_sources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_semantic_entities_DataSourceId_PhysicalTableName",
                schema: "semantic",
                table: "semantic_entities",
                columns: new[] { "DataSourceId", "PhysicalTableName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_semantic_fields_EntityId_PhysicalColumnName",
                schema: "semantic",
                table: "semantic_fields",
                columns: new[] { "EntityId", "PhysicalColumnName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "semantic_fields",
                schema: "semantic");

            migrationBuilder.DropTable(
                name: "sync_runs",
                schema: "semantic");

            migrationBuilder.DropTable(
                name: "semantic_entities",
                schema: "semantic");

            migrationBuilder.DropTable(
                name: "data_sources",
                schema: "semantic");
        }
    }
}
