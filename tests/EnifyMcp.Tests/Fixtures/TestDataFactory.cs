using EnifyMcp.Core.Models;
using EnifyMcp.Core.Services;
using Moq;

namespace EnifyMcp.Tests.Fixtures;

public static class TestDataFactory
{
    #region WorkspaceInfo Factory

    public static WorkspaceInfo CreateWorkspace(
        string id = "ws-001",
        string name = "Test Workspace")
    {
        return new WorkspaceInfo(id, name);
    }

    public static IReadOnlyList<WorkspaceInfo> CreateTypicalWorkspaceSet()
    {
        return new List<WorkspaceInfo>
        {
            CreateWorkspace("ws-001", "Development"),
            CreateWorkspace("ws-002", "Production"),
            CreateWorkspace("ws-003", "Testing")
        };
    }

    #endregion

    #region BoardInfo Factory

    public static BoardInfo CreateBoard(
        string id = "board-001",
        string name = "Test Board",
        string workspaceId = "ws-001",
        string? workspaceName = "Test Workspace",
        bool isFavorite = false)
    {
        return new BoardInfo(id, name, workspaceId, workspaceName, isFavorite);
    }

    public static IReadOnlyList<BoardInfo> CreateTypicalBoardSet()
    {
        return new List<BoardInfo>
        {
            CreateBoard("board-001", "VG News", "ws-001", "Development", true),
            CreateBoard("board-002", "BT Sports", "ws-001", "Development", false),
            CreateBoard("board-003", "Aftenposten", "ws-002", "Production", true),
            CreateBoard("board-004", "E24 Finance", "ws-002", "Production", false),
            CreateBoard("board-005", "Test Board", "ws-003", "Testing", false)
        };
    }

    public static IReadOnlyList<BoardInfo> CreateBoardsForWorkspace(string workspaceId, string workspaceName, int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateBoard(
                id: $"{workspaceId}:board-{i:D3}",
                name: $"Board {i}",
                workspaceId: workspaceId,
                workspaceName: workspaceName,
                isFavorite: i == 1))
            .ToList();
    }

    #endregion

    #region Mock Service Factories

    public static Mock<IEnifyService> CreateMockEnifyService(
        bool isInstalled = true,
        IReadOnlyList<WorkspaceInfo>? workspaces = null,
        IReadOnlyList<BoardInfo>? boards = null)
    {
        var mock = new Mock<IEnifyService>();
        var testWorkspaces = workspaces ?? CreateTypicalWorkspaceSet();
        var testBoards = boards ?? CreateTypicalBoardSet();

        mock.Setup(s => s.IsEnifyInstalled).Returns(isInstalled);

        mock.Setup(s => s.GetWorkspacesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(isInstalled ? testWorkspaces : Array.Empty<WorkspaceInfo>());

        mock.Setup(s => s.GetBoardsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(isInstalled ? testBoards : Array.Empty<BoardInfo>());

        mock.Setup(s => s.GetRecentBoardsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(isInstalled ? testBoards.Take(2).ToList() : Array.Empty<BoardInfo>());

        mock.Setup(s => s.StartBoardAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string boardId, CancellationToken _) =>
                isInstalled && !string.IsNullOrWhiteSpace(boardId));

        mock.Setup(s => s.StopBoardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(isInstalled);

        mock.Setup(s => s.RefreshBoardAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(isInstalled);

        mock.Setup(s => s.RestoreDisplaysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(isInstalled);

        return mock;
    }

    public static Mock<IEnifyService> CreateMockEnifyServiceNotInstalled()
    {
        return CreateMockEnifyService(isInstalled: false);
    }

    #endregion
}
