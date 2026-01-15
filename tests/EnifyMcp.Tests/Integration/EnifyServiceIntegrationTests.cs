using Microsoft.Extensions.Logging.Abstractions;

namespace EnifyMcp.Tests.Integration;

/// <summary>
/// Integration tests that interact with the real Enify CLI.
/// These tests will be skipped if Enify is not installed on the machine.
/// </summary>
public class EnifyServiceIntegrationTests
{
    private readonly EnifyService _service;

    public EnifyServiceIntegrationTests()
    {
        _service = new EnifyService(NullLogger<EnifyService>.Instance);
    }

    [Fact]
    public void EnifyService_Constructor_DoesNotThrow()
    {
        // Act
        Action act = () => new EnifyService(NullLogger<EnifyService>.Instance);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task GetWorkspacesAsync_ReturnsData_WhenEnifyInstalled()
    {
        // Skip if Enify not installed
        if (!_service.IsEnifyInstalled)
        {
            return;
        }

        // Act
        var result = await _service.GetWorkspacesAsync();

        // Assert
        result.Should().NotBeNull();
        // May be empty if no workspaces configured, but should not throw
    }

    [Fact]
    public async Task GetBoardsAsync_ReturnsData_WhenEnifyInstalled()
    {
        // Skip if Enify not installed
        if (!_service.IsEnifyInstalled)
        {
            return;
        }

        // Act
        var result = await _service.GetBoardsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRecentBoardsAsync_ReturnsData_WhenEnifyInstalled()
    {
        // Skip if Enify not installed
        if (!_service.IsEnifyInstalled)
        {
            return;
        }

        // Act
        var result = await _service.GetRecentBoardsAsync();

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkspacesAsync_ReturnsEmpty_WhenEnifyNotInstalled()
    {
        // Skip if Enify IS installed (we want to test the not-installed case)
        if (_service.IsEnifyInstalled)
        {
            return;
        }

        // Act
        var result = await _service.GetWorkspacesAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task StartBoardAsync_ReturnsFalse_WhenBoardIdEmpty()
    {
        // This test should work regardless of whether Enify is installed
        // Act
        var result = await _service.StartBoardAsync("");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task StartBoardAsync_ReturnsFalse_WhenBoardIdWhitespace()
    {
        // Act
        var result = await _service.StartBoardAsync("   ");

        // Assert
        result.Should().BeFalse();
    }
}
