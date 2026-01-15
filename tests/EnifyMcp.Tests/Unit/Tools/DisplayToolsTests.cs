using EnifyMcp.Tools;

namespace EnifyMcp.Tests.Unit.Tools;

public class DisplayToolsTests
{
    private readonly Mock<IEnifyService> _mockEnifyService;

    public DisplayToolsTests()
    {
        _mockEnifyService = TestDataFactory.CreateMockEnifyService();
    }

    private DisplayTools CreateSut() => new(_mockEnifyService.Object);

    #region RestoreDisplays Tests

    [Fact]
    public async Task RestoreDisplays_ReturnsSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = await sut.RestoreDisplays();

        // Assert
        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task RestoreDisplays_ReturnsError_WhenEnifyNotInstalled()
    {
        // Arrange
        var mockService = TestDataFactory.CreateMockEnifyServiceNotInstalled();
        var sut = new DisplayTools(mockService.Object);

        // Act
        var result = await sut.RestoreDisplays();

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("EnifyNotInstalled");
        result.Error.Should().Contain("not installed");
    }

    [Fact]
    public async Task RestoreDisplays_CallsService_ExactlyOnce()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        await sut.RestoreDisplays();

        // Assert
        _mockEnifyService.Verify(
            s => s.RestoreDisplaysAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RestoreDisplays_ReturnsError_WhenServiceFails()
    {
        // Arrange
        var mockService = new Mock<IEnifyService>();
        mockService.Setup(s => s.IsEnifyInstalled).Returns(true);
        mockService.Setup(s => s.RestoreDisplaysAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var sut = new DisplayTools(mockService.Object);

        // Act
        var result = await sut.RestoreDisplays();

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorCode.Should().Be("CommandFailed");
    }

    #endregion
}
