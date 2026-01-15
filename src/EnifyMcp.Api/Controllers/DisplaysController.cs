using EnifyMcp.Core.Models.Results;
using EnifyMcp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnifyMcp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DisplaysController : ControllerBase
{
    private readonly IEnifyService _enifyService;

    public DisplaysController(IEnifyService enifyService)
    {
        _enifyService = enifyService;
    }

    /// <summary>
    /// Restore display/monitor configuration
    /// </summary>
    /// <returns>Operation result</returns>
    [HttpPost("restore")]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> RestoreDisplays(CancellationToken cancellationToken)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return StatusCode(503, new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: "EnifyNotInstalled"
            ));
        }

        var success = await _enifyService.RestoreDisplaysAsync(cancellationToken);

        return Ok(new OperationResult(
            Success: success,
            Error: success ? null : "Failed to restore displays",
            ErrorCode: success ? null : "CommandFailed"
        ));
    }
}
