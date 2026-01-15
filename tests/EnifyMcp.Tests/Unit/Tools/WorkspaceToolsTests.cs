using EnifyMcp.Tools;

namespace EnifyMcp.Tests.Unit.Tools;

public class WorkspaceToolsTests
{
    private readonly Mock<IEnifyService> _mockEnifyService;

    public WorkspaceToolsTests()
    {
        _mockEnifyService = TestDataFactory.CreateMockEnifyService();
    }

    private WorkspaceTools CreateSut() => new(_mockEnifyService.Object);

    #region ListWorkspaces Tests

    [Fact]
    public async Task ListWorkspaces_ReturnsWorkspaces_WhenEnifyInstalled()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.ListWorkspaces();

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(3);
        result.Should().Contain(w => w.Name == "Development");
    }

    [Fact]
    public async Task ListWorkspaces_ReturnsEmpty_WhenEnifyNotInstalled()
    {
        // Arrange
        var mockService = TestDataFactory.CreateMockEnifyServiceNotInstalled();
        var sut = new WorkspaceTools(mockService.Object);

        // Act
        var result = await sut.ListWorkspaces();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ListWorkspaces_CallsService_ExactlyOnce()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        await sut.ListWorkspaces();

        // Assert
        _mockEnifyService.Verify(
            s => s.GetWorkspacesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ListWorkspaces_MapsToDto_Correctly()
    {
        // Arrange
        var workspaces = new List<WorkspaceInfo>
        {
            new("id-123", "Custom Workspace")
        };
        var mockService = TestDataFactory.CreateMockEnifyService(workspaces: workspaces);
        var sut = new WorkspaceTools(mockService.Object);

        // Act
        var result = await sut.ListWorkspaces();

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be("id-123");
        result[0].Name.Should().Be("Custom Workspace");
    }

    #endregion
}
