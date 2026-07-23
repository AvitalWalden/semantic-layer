using Microsoft.AspNetCore.Mvc;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Dtos;

namespace SemanticLayer.Api.Controllers;

[ApiController]
[Route("api/sync")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    private readonly IMetadataMergeService _metadataMergeService;

    public SyncController(ISyncService syncService, IMetadataMergeService metadataMergeService)
    {
        _syncService = syncService;
        _metadataMergeService = metadataMergeService;
    }

    /// <summary>Runs a non-destructive schema sync against the physical source.</summary>
    [HttpPost("schema")]
    public async Task<ActionResult<SyncResultDto>> SyncSchema(CancellationToken ct)
        => Ok(await _syncService.SyncSchemaAsync(ct));

    /// <summary>Merges an uploaded metadata file (JSON) into the semantic layer.</summary>
    [HttpPost("metadata")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<SyncResultDto>> MergeMetadata(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("A non-empty metadata file is required.");

        try
        {
            await using var stream = file.OpenReadStream();
            var result = await _metadataMergeService.MergeAsync(stream, ct);
            return Ok(result);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
