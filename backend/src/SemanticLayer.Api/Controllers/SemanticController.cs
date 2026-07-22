using Microsoft.AspNetCore.Mvc;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Dtos;

namespace SemanticLayer.Api.Controllers;

[ApiController]
[Route("api/semantic")]
public class SemanticController : ControllerBase
{
    private readonly ISemanticService _service;

    public SemanticController(ISemanticService service) => _service = service;

    /// <summary>Lists semantic entities. Set onlyVisible=true for the business view.</summary>
    [HttpGet("entities")]
    public async Task<ActionResult<IReadOnlyList<EntityDto>>> GetEntities(
        [FromQuery] bool onlyVisible = false, CancellationToken ct = default)
        => Ok(await _service.GetEntitiesAsync(onlyVisible, ct));

    /// <summary>Gets a single entity with its fields.</summary>
    [HttpGet("entities/{id:int}")]
    public async Task<ActionResult<EntityDetailDto>> GetEntity(int id, CancellationToken ct)
    {
        var entity = await _service.GetEntityAsync(id, ct);
        return entity is null ? NotFound() : Ok(entity);
    }

    /// <summary>Updates the business attributes of an entity (marks it user-modified).</summary>
    [HttpPut("entities/{id:int}")]
    public async Task<ActionResult<EntityDetailDto>> UpdateEntity(int id, [FromBody] UpdateEntityDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.BusinessName))
            return BadRequest("BusinessName is required.");

        var updated = await _service.UpdateEntityAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Updates the business attributes of a field (marks it user-modified).</summary>
    [HttpPut("fields/{id:int}")]
    public async Task<ActionResult<FieldDto>> UpdateField(int id, [FromBody] UpdateFieldDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.BusinessName))
            return BadRequest("BusinessName is required.");

        var updated = await _service.UpdateFieldAsync(id, dto, ct);
        return updated is null ? NotFound() : Ok(updated);
    }
}
