using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using EnifyMcp.Core.Models;
using Microsoft.Extensions.Logging;

namespace EnifyMcp.Core.Services;

public partial class EnifyService : IEnifyService
{
    private static readonly string[] PossiblePaths =
    [
        "enify",
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft", "WindowsApps", "enify.exe"),
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dotnet", "tools", "enify.exe"),
    ];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(10);

    private readonly ILogger<EnifyService> _logger;
    private readonly string? _enifyPath;

    public EnifyService(ILogger<EnifyService> logger)
    {
        _logger = logger;
        _enifyPath = FindEnifyPath();

        if (_enifyPath == null)
        {
            _logger.LogWarning("Enify executable not found in any known location");
        }
        else
        {
            _logger.LogInformation("Found Enify at: {Path}", _enifyPath);
        }
    }

    public bool IsEnifyInstalled => _enifyPath != null;

    public async Task<IReadOnlyList<WorkspaceInfo>> GetWorkspacesAsync(CancellationToken cancellationToken = default)
    {
        var output = await ExecuteCommandAsync("get-workspaces", cancellationToken);
        if (output == null)
        {
            return [];
        }

        try
        {
            var dtos = JsonSerializer.Deserialize<List<WorkspaceDto>>(output, JsonOptions);
            return dtos?.Select(d => new WorkspaceInfo(d.Id, d.Name)).ToList() ?? [];
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse workspaces JSON");
            return [];
        }
    }

    public async Task<IReadOnlyList<BoardInfo>> GetBoardsAsync(CancellationToken cancellationToken = default)
    {
        var workspaces = await GetWorkspacesAsync(cancellationToken);
        var workspaceMap = workspaces.ToDictionary(w => w.Id, w => w.Name);

        var output = await ExecuteCommandAsync("get-boards", cancellationToken);
        if (output == null)
        {
            return [];
        }

        try
        {
            var dtos = JsonSerializer.Deserialize<List<BoardDto>>(output, JsonOptions);
            return dtos?.Select(d => new BoardInfo(
                d.Id,
                d.Name,
                d.WorkspaceId,
                workspaceMap.GetValueOrDefault(d.WorkspaceId),
                d.Favorite
            )).ToList() ?? [];
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse boards JSON");
            return [];
        }
    }

    public async Task<IReadOnlyList<BoardInfo>> GetRecentBoardsAsync(CancellationToken cancellationToken = default)
    {
        var workspaces = await GetWorkspacesAsync(cancellationToken);
        var workspaceMap = workspaces.ToDictionary(w => w.Id, w => w.Name);

        var output = await ExecuteCommandAsync("get-recent-list", cancellationToken);
        if (output == null)
        {
            return [];
        }

        try
        {
            var dtos = JsonSerializer.Deserialize<List<BoardDto>>(output, JsonOptions);
            return dtos?.Select(d => new BoardInfo(
                d.Id,
                d.Name,
                d.WorkspaceId,
                workspaceMap.GetValueOrDefault(d.WorkspaceId),
                d.Favorite
            )).ToList() ?? [];
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse recent boards JSON");
            return [];
        }
    }

    public async Task<bool> StartBoardAsync(string boardId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(boardId))
        {
            _logger.LogWarning("StartBoard called with empty boardId");
            return false;
        }

        return await ExecuteCommandFireAndForgetAsync($"start-board {boardId}", cancellationToken);
    }

    public async Task<bool> StopBoardAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteCommandFireAndForgetAsync("stop-board", cancellationToken);
    }

    public async Task<bool> RefreshBoardAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteCommandFireAndForgetAsync("refresh-board", cancellationToken);
    }

    public async Task<bool> RestoreDisplaysAsync(CancellationToken cancellationToken = default)
    {
        return await ExecuteCommandFireAndForgetAsync("restore-displays", cancellationToken);
    }

    private static string? FindEnifyPath()
    {
        foreach (var path in PossiblePaths)
        {
            if (path == "enify")
            {
                var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
                var paths = pathEnv.Split(Path.PathSeparator);

                foreach (var dir in paths)
                {
                    var fullPath = Path.Combine(dir, "enify.exe");
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }
            }
            else if (File.Exists(path))
            {
                return path;
            }
        }

        return null;
    }

    private async Task<string?> ExecuteCommandAsync(string command, CancellationToken cancellationToken)
    {
        if (_enifyPath == null)
        {
            return null;
        }

        _logger.LogDebug("Executing enify command: {Command}", command);

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = _enifyPath,
                Arguments = command,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(CommandTimeout);

            var outputTask = process.StandardOutput.ReadToEndAsync(cts.Token);
            var errorTask = process.StandardError.ReadToEndAsync(cts.Token);

            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Command timed out: {Command}", command);
                try { process.Kill(); } catch { }
                return null;
            }

            var output = await outputTask;
            var error = await errorTask;

            _logger.LogDebug("Command exit code: {ExitCode}, Output length: {Length}",
                process.ExitCode, output.Length);

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Command stderr: {Error}", error);
            }

            // Strip ANSI codes and normalize line breaks
            output = StripAnsiCodes(output);
            output = output.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");

            return output;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command: {Command}", command);
            return null;
        }
    }

    private async Task<bool> ExecuteCommandFireAndForgetAsync(string command, CancellationToken cancellationToken)
    {
        if (_enifyPath == null)
        {
            return false;
        }

        _logger.LogDebug("Executing enify command (fire-and-forget): {Command}", command);

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = _enifyPath,
                Arguments = command,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                _logger.LogWarning("Failed to start process for command: {Command}", command);
                return false;
            }

            // Wait briefly to see if it starts successfully
            await Task.Delay(100, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command: {Command}", command);
            return false;
        }
    }

    private static string StripAnsiCodes(string input)
    {
        return AnsiRegex().Replace(input, string.Empty);
    }

    [GeneratedRegex(@"\x1B\[[0-9;]*m")]
    private static partial Regex AnsiRegex();

    // Internal DTOs for JSON deserialization
    private sealed class WorkspaceDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    private sealed class BoardDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string WorkspaceId { get; set; } = string.Empty;
        public bool Favorite { get; set; }
    }
}
