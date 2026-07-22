using System.Text.Json.Serialization;
using SemanticLayer.Api;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Configuration;
using SemanticLayer.Application.Services;
using SemanticLayer.Infrastructure;
using SemanticLayer.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "frontend";

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SemanticLayerOptions>(
    builder.Configuration.GetSection(SemanticLayerOptions.SectionName));

// Infrastructure (DbContext, Npgsql, repository, introspection, data query).
builder.Services.AddInfrastructure(builder.Configuration);

// Application services.
builder.Services.AddScoped<ISemanticService, SemanticService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IMetadataMergeService, MetadataMergeService>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options => options.AddPolicy(CorsPolicy, policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(CorsPolicy);
app.MapControllers();

// Apply migrations and optionally perform an initial schema sync on startup.
await StartupInitializer.RunAsync(app);

app.Run();
