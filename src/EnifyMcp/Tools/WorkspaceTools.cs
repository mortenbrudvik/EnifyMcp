using System.ComponentModel;
using EnifyMcp.Core.Models;
using EnifyMcp.Core.Models.Results;
using EnifyMcp.Core.Services;
using ModelContextProtocol.Server;

namespace EnifyMcp.Tools;

[McpServerToolType]
public class WorkspaceTools
{
    private readonly IEnifyService _enifyService;

    public WorkspaceTools(IEnifyService enifyService)
    {
        _enifyService = enifyService;
    }

    [McpServerTool(Name = "list_workspaces")]
    [Description("List all Enify workspaces. Returns a list of workspaces with their IDs and names.")]
    public async Task<IReadOnlyList<WorkspaceDto>> ListWorkspaces(CancellationToken cancellationToken = default)
    {
        if (!_enifyService.IsEnifyInstalled)
        {
            return [];
        }

        var workspaces = await _enifyService.GetWorkspacesAsync(cancellationToken);
        return workspaces.Select(w => new WorkspaceDto(w.Id, w.Name)).ToList();
    }
}
