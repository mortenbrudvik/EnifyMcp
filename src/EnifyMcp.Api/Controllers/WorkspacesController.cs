using EnifyMcp.Core.Models.Results;
using EnifyMcp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnifyMcp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WorkspacesController : ControllerBase
{
    private readonly IEnifyService _enifyService;

    public WorkspacesController(IEnifyService enifyService)
    {
        _enifyService = enifyService;
    }

    /// <summary>
    /// List all Enify workspaces
    /// </summary>
    /// <returns>List of workspaces with their IDs and names</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WorkspaceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetWorkspaces(CancellationToken cancellationToken)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return StatusCode(503, new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: "EnifyNotInstalled"
            ));
        }

        var workspaces = await _enifyService.GetWorkspacesAsync(cancellationToken);
        var dtos = workspaces.Select(w => new WorkspaceDto(w.Id, w.Name)).ToList();
        return Ok(dtos);
    }
}
