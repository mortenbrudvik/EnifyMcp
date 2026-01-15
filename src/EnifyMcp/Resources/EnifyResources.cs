using System.ComponentModel;
using System.Text.Json;
using EnifyMcp.Core.Services;
using ModelContextProtocol.Server;

namespace EnifyMcp.Resources;

[McpServerResourceType]
public class EnifyResources
{
    private readonly IEnifyService _enifyService;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public EnifyResources(IEnifyService enifyService)
    {
        _enifyService = enifyService;
    }

    [McpServerResource(Name = "enify://workspaces")]
    [Description("Browse all Enify workspaces. Returns a list of workspace IDs and names.")]
    public async Task<string> GetWorkspaces(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return JsonSerializer.Serialize(new { error = "Enify is not installed" }, JsonOptions);
        }

        var workspaces = await _enifyService.GetWorkspacesAsync(cancellationToken);
        return JsonSerializer.Serialize(workspaces, JsonOptions);
    }

    [McpServerResource(Name = "enify://boards")]
    [Description("Browse all Enify boards across all workspaces. Returns board details including workspace info and favorite status.")]
    public async Task<string> GetAllBoards(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return JsonSerializer.Serialize(new { error = "Enify is not installed" }, JsonOptions);
        }

        var boards = await _enifyService.GetBoardsAsync(cancellationToken);
        return JsonSerializer.Serialize(boards, JsonOptions);
    }

    [McpServerResource(Name = "enify://boards/favorites")]
    [Description("Browse favorite Enify boards only. Returns boards marked as favorites.")]
    public async Task<string> GetFavoriteBoards(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return JsonSerializer.Serialize(new { error = "Enify is not installed" }, JsonOptions);
        }

        var boards = await _enifyService.GetBoardsAsync(cancellationToken);
        var favorites = boards.Where(b => b.IsFavorite).ToList();
        return JsonSerializer.Serialize(favorites, JsonOptions);
    }

    [McpServerResource(Name = "enify://boards/recent")]
    [Description("Browse recently used Enify boards. Returns boards sorted by last use.")]
    public async Task<string> GetRecentBoards(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return JsonSerializer.Serialize(new { error = "Enify is not installed" }, JsonOptions);
        }

        var boards = await _enifyService.GetRecentBoardsAsync(cancellationToken);
        return JsonSerializer.Serialize(boards, JsonOptions);
    }

    [McpServerResource(Name = "enify://status")]
    [Description("Get Enify installation and availability status.")]
    public Task<string> GetStatus(CancellationToken cancellationToken = default)
    {
        var status = new
        {
            isInstalled = _enifyService.IsEnifyInstalled,
            status = _enifyService.IsEnifyInstalled ? "ready" : "not_installed",
            message = _enifyService.IsEnifyInstalled
                ? "Enify is installed and ready"
                : "Enify is not installed or not found in PATH"
        };
        return Task.FromResult(JsonSerializer.Serialize(status, JsonOptions));
    }
}
