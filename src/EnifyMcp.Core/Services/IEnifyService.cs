using EnifyMcp.Core.Models;

namespace EnifyMcp.Core.Services;

public interface IEnifyService
{
    bool IsEnifyInstalled { get; }

    Task<IReadOnlyList<WorkspaceInfo>> GetWorkspacesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BoardInfo>> GetBoardsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BoardInfo>> GetRecentBoardsAsync(CancellationToken cancellationToken = default);

    Task<bool> StartBoardAsync(string boardId, CancellationToken cancellationToken = default);

    Task<bool> StopBoardAsync(CancellationToken cancellationToken = default);

    Task<bool> RefreshBoardAsync(CancellationToken cancellationToken = default);

    Task<bool> RestoreDisplaysAsync(CancellationToken cancellationToken = default);
}
