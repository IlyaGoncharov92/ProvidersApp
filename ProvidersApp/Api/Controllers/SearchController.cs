using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController(ISearchService _searchService) : ControllerBase
{
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var response = await _searchService.SearchAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping(CancellationToken cancellationToken)
    {
        var isAvailable = await _searchService.IsAvailableAsync(cancellationToken);
        return isAvailable ? Ok() : StatusCode(500);
    }
}