using EnifyMcp.Tools;

namespace EnifyMcp.Tests.Unit.Tools;

public class BoardToolsTests
{
    private readonly Mock<IEnifyService> _mockEnifyService;

    public BoardToolsTests()
    {
        _mockEnifyService = TestDataFactory.CreateMockEnifyService();
    }

    private BoardTools CreateSut() => new(_mockEnifyService.Object);

    #region ListBoards Tests

    [Fact]
    public async Task ListBoards_ReturnsAllBoards_WhenNoFilters()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.ListBoards();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task ListBoards_ReturnsEmpty_WhenEnifyNotInstalled()
    {
        // Arrange
        var mockService = TestDataFactory.CreateMockEnifyServiceNotInstalled();
        var sut = new BoardTools(mockService.Object);

        // Act
        var result = await sut.ListBoards();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ListBoards_FiltersByWorkspaceId_WhenProvided()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.ListBoards(workspaceId: "ws-001");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(b => b.WorkspaceId == "ws-001");
    }

    [Fact]
    public async Task ListBoards_FiltersFavoritesOnly_WhenTrue()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.ListBoards(favoritesOnly: true);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(b => b.IsFavorite);
    }

    [Fact]
    public async Task ListBoards_CombinesFilters_WhenBothProvided()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.ListBoards(workspaceId: "ws-001", favoritesOnly: true);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("VG News");
    }

    #endregion

    #region GetRecentBoards Tests

    [Fact]
    public async Task GetRecentBoards_ReturnsBoards_WhenEnifyInstalled()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.GetRecentBoards();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetRecentBoards_ReturnsEmpty_WhenEnifyNotInstalled()
    {
        // Arrange
        var mockService = TestDataFactory.CreateMockEnifyServiceNotInstalled();
        var sut = new BoardTools(mockService.Object);

        // Act
        var result = await sut.GetRecentBoards();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region StartBoard Tests

    [Fact]
    public async Task StartBoard_ReturnsSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.StartBoard("board-001");

        // Assert
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task StartBoard_ReturnsError_WhenEnifyNotInstalled()
    {
        // Arrange
        var mockService = TestDataFactory.CreateMockEnifyServiceNotInstalled();
        var sut = new BoardTools(mockService.Object);

        // Act
        var result = await sut.StartBoard("board-001");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("EnifyNotInstalled");
    }

    [Fact]
    public async Task StartBoard_ReturnsError_WhenBoardIdEmpty()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.StartBoard("");

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("InvalidBoardId");
    }

    [Fact]
    public async Task StartBoard_ReturnsError_WhenBoardIdNull()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.StartBoard(null!);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("InvalidBoardId");
    }

    [Fact]
    public async Task StartBoard_CallsService_WithCorrectBoardId()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        await sut.StartBoard("my-board-id");

        // Assert
        _mockEnifyService.Verify(
            s => s.StartBoardAsync("my-board-id", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region StopBoard Tests

    [Fact]
    public async Task StopBoard_ReturnsSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.StopBoard();

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task StopBoard_ReturnsError_WhenEnifyNotInstalled()
    {
        // Arrange
        var mockService = TestDataFactory.CreateMockEnifyServiceNotInstalled();
        var sut = new BoardTools(mockService.Object);

        // Act
        var result = await sut.StopBoard();

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("EnifyNotInstalled");
    }

    [Fact]
    public async Task StopBoard_CallsService_ExactlyOnce()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        await sut.StopBoard();

        // Assert
        _mockEnifyService.Verify(
            s => s.StopBoardAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region RefreshBoard Tests

    [Fact]
    public async Task RefreshBoard_ReturnsSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.RefreshBoard();

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshBoard_ReturnsError_WhenEnifyNotInstalled()
    {
        // Arrange
        var mockService = TestDataFactory.CreateMockEnifyServiceNotInstalled();
        var sut = new BoardTools(mockService.Object);

        // Act
        var result = await sut.RefreshBoard();

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("EnifyNotInstalled");
    }

    #endregion
}
