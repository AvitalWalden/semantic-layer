using Microsoft.AspNetCore.Mvc;
using SemanticLayer.Application.Abstractions;
using SemanticLayer.Application.Dtos;

namespace SemanticLayer.Api.Controllers;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private readonly IDataQueryService _dataQueryService;

    public DataController(IDataQueryService dataQueryService) => _dataQueryService = dataQueryService;

    /// <summary>
    /// Returns a page of data for an entity, viewed through the semantic layer:
    /// only visible fields, business column names, derived fields included.
    /// </summary>
    [HttpGet("{entityId:int}")]
    public async Task<ActionResult<DataResultDto>> GetData(
        int entityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken ct = default)
    {
        try
        {
            return Ok(await _dataQueryService.GetDataAsync(entityId, page, pageSize, ct));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
