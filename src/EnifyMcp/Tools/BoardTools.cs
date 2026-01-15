using System.ComponentModel;
using EnifyMcp.Core.Models;
using EnifyMcp.Core.Models.Results;
using EnifyMcp.Core.Services;
using ModelContextProtocol.Server;

namespace EnifyMcp.Tools;

[McpServerToolType]
public class BoardTools
{
    private readonly IEnifyService _enifyService;

    public BoardTools(IEnifyService enifyService)
    {
        _enifyService = enifyService;
    }

    [McpServerTool(Name = "list_boards")]
    [Description("List all Enify boards. Returns boards with their IDs, names, workspace info, and favorite status.")]
    public async Task<IReadOnlyList<BoardDto>> ListBoards(
        [Description("Filter by workspace ID (optional)")]
        string? workspaceId = null,
        [Description("Only return favorite boards (default: false)")]
        bool favoritesOnly = false,
        CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return [];
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

        return boards.Select(b => new BoardDto(
            b.Id,
            b.Name,
            b.WorkspaceId,
            b.WorkspaceName,
            b.IsFavorite
        )).ToList();
    }

    [McpServerTool(Name = "get_recent_boards")]
    [Description("Get recently used Enify boards.")]
    public async Task<IReadOnlyList<BoardDto>> GetRecentBoards(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return [];
        }

        var boards = await _enifyService.GetRecentBoardsAsync(cancellationToken);
        return boards.Select(b => new BoardDto(
            b.Id,
            b.Name,
            b.WorkspaceId,
            b.WorkspaceName,
            b.IsFavorite
        )).ToList();
    }

    [McpServerTool(Name = "start_board")]
    [Description("Start a specific Enify board by its ID. The board will launch and arrange windows according to its configuration.")]
    public async Task<OperationResult> StartBoard(
        [Description("The board ID to start (required)")]
        string boardId,
        CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: nameof(EnifyErrorCode.EnifyNotInstalled)
            );
        }

        if (string.IsNullOrWhiteSpace(boardId))
        {
            return new OperationResult(
                Success: false,
                Error: "Board ID is required",
                ErrorCode: nameof(EnifyErrorCode.InvalidBoardId)
            );
        }

        var success = await _enifyService.StartBoardAsync(boardId, cancellationToken);

        return new OperationResult(
            Success: success,
            Error: success ? null : "Failed to start board",
            ErrorCode: success ? null : nameof(EnifyErrorCode.CommandFailed)
        );
    }

    [McpServerTool(Name = "stop_board")]
    [Description("Stop the currently running Enify board.")]
    public async Task<OperationResult> StopBoard(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: nameof(EnifyErrorCode.EnifyNotInstalled)
            );
        }

        var success = await _enifyService.StopBoardAsync(cancellationToken);

        return new OperationResult(
            Success: success,
            Error: success ? null : "Failed to stop board",
            ErrorCode: success ? null : nameof(EnifyErrorCode.CommandFailed)
        );
    }

    [McpServerTool(Name = "refresh_board")]
    [Description("Refresh the currently running Enify board. This will re-apply the board's window arrangement.")]
    public async Task<OperationResult> RefreshBoard(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: nameof(EnifyErrorCode.EnifyNotInstalled)
            );
        }

        var success = await _enifyService.RefreshBoardAsync(cancellationToken);

        return new OperationResult(
            Success: success,
            Error: success ? null : "Failed to refresh board",
            ErrorCode: success ? null : nameof(EnifyErrorCode.CommandFailed)
        );
    }
}
