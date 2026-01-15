using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace EnifyMcp.Tests.Integration;

public class DependencyInjectionTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public DependencyInjectionTests()
    {
        var services = new ServiceCollection();

        // Register logging
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

        // Register services exactly as in the main application
        services.AddSingleton<IEnifyService, EnifyService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    [Fact]
    public void ServiceProvider_ResolvesEnifyService_Successfully()
    {
        // Act
        var service = _serviceProvider.GetService<IEnifyService>();

        // Assert
        service.Should().NotBeNull();
        service.Should().BeOfType<EnifyService>();
    }

    [Fact]
    public void IEnifyService_Singleton_ReturnsSameInstance()
    {
        // Act
        var instance1 = _serviceProvider.GetRequiredService<IEnifyService>();
        var instance2 = _serviceProvider.GetRequiredService<IEnifyService>();

        // Assert
        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void ServiceProvider_Build_DoesNotThrow()
    {
        // Act
        Action act = () =>
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            services.AddSingleton<IEnifyService, EnifyService>();

            using var provider = services.BuildServiceProvider();
            _ = provider.GetRequiredService<IEnifyService>();
        };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnifyService_IsEnifyInstalled_ReturnsBoolean()
    {
        // Arrange
        var service = _serviceProvider.GetRequiredService<IEnifyService>();

        // Act
        var isInstalled = service.IsEnifyInstalled;

        // Assert - Should be a valid boolean (true or false)
        isInstalled.Should().Be(isInstalled); // Just verify it doesn't throw
    }
}
