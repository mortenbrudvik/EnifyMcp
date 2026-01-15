using EnifyMcp.Core.Models;
using EnifyMcp.Core.Models.Results;
using EnifyMcp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnifyMcp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BoardsController : ControllerBase
{
    private readonly IEnifyService _enifyService;

    public BoardsController(IEnifyService enifyService)
    {
        _enifyService = enifyService;
    }

    /// <summary>
    /// List all Enify boards
    /// </summary>
    /// <param name="workspaceId">Optional: Filter by workspace ID</param>
    /// <param name="favoritesOnly">Optional: Only return favorite boards</param>
    /// <returns>List of boards with their details</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BoardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetBoards(
        [FromQuery] string? workspaceId = null,
        [FromQuery] bool favoritesOnly = false,
        CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return StatusCode(503, new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: "EnifyNotInstalled"
            ));
        }

        var boards = await _enifyService.GetBoardsAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(workspaceId))
        {
            boards = boards.Where(b => b.WorkspaceId == workspaceId).ToList();
        }

        if (favoritesOnly)
        {
            boards = boards.Where(b => b.IsFavorite).ToList();
        }

        var dtos = boards.Select(b => new BoardDto(
            b.Id, b.Name, b.WorkspaceId, b.WorkspaceName, b.IsFavorite
        )).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Get recently used boards
    /// </summary>
    /// <returns>List of recently used boards</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IReadOnlyList<BoardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetRecentBoards(CancellationToken cancellationToken)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return StatusCode(503, new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: "EnifyNotInstalled"
            ));
        }

        var boards = await _enifyService.GetRecentBoardsAsync(cancellationToken);
        var dtos = boards.Select(b => new BoardDto(
            b.Id, b.Name, b.WorkspaceId, b.WorkspaceName, b.IsFavorite
        )).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Start a specific board by its ID
    /// </summary>
    /// <param name="boardId">The board ID to start</param>
    /// <returns>Operation result</returns>
    [HttpPost("{boardId}/start")]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> StartBoard(string boardId, CancellationToken cancellationToken)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return StatusCode(503, new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: "EnifyNotInstalled"
            ));
        }

        if (string.IsNullOrWhiteSpace(boardId))
        {
            return BadRequest(new OperationResult(
                Success: false,
                Error: "Board ID is required",
                ErrorCode: "InvalidBoardId"
            ));
        }

        var success = await _enifyService.StartBoardAsync(boardId, cancellationToken);

        return Ok(new OperationResult(
            Success: success,
            Error: success ? null : "Failed to start board",
            ErrorCode: success ? null : "CommandFailed"
        ));
    }

    /// <summary>
    /// Stop the currently running board
    /// </summary>
    /// <returns>Operation result</returns>
    [HttpPost("stop")]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> StopBoard(CancellationToken cancellationToken)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return StatusCode(503, new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: "EnifyNotInstalled"
            ));
        }

        var success = await _enifyService.StopBoardAsync(cancellationToken);

        return Ok(new OperationResult(
            Success: success,
            Error: success ? null : "Failed to stop board",
            ErrorCode: success ? null : "CommandFailed"
        ));
    }

    /// <summary>
    /// Refresh the currently running board
    /// </summary>
    /// <returns>Operation result</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> RefreshBoard(CancellationToken cancellationToken)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return StatusCode(503, new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: "EnifyNotInstalled"
            ));
        }

        var success = await _enifyService.RefreshBoardAsync(cancellationToken);

        return Ok(new OperationResult(
            Success: success,
            Error: success ? null : "Failed to refresh board",
            ErrorCode: success ? null : "CommandFailed"
        ));
    }
}
