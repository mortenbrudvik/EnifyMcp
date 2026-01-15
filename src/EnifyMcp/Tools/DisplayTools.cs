using System.ComponentModel;
using EnifyMcp.Core.Models;
using EnifyMcp.Core.Models.Results;
using EnifyMcp.Core.Services;
using ModelContextProtocol.Server;

namespace EnifyMcp.Tools;

[McpServerToolType]
public class DisplayTools
{
    private readonly IEnifyService _enifyService;

    public DisplayTools(IEnifyService enifyService)
    {
        _enifyService = enifyService;
    }

    [McpServerTool(Name = "restore_displays")]
    [Description("Restore display/monitor configuration to the state saved by Enify.")]
    public async Task<OperationResult> RestoreDisplays(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return new OperationResult(
                Success: false,
                Error: "Enify is not installed or not found in PATH",
                ErrorCode: nameof(EnifyErrorCode.EnifyNotInstalled)
            );
        }

        var success = await _enifyService.RestoreDisplaysAsync(cancellationToken);

        return new OperationResult(
            Success: success,
            Error: success ? null : "Failed to restore displays",
            ErrorCode: success ? null : nameof(EnifyErrorCode.CommandFailed)
        );
    }
}
