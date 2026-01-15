namespace EnifyMcp.Core.Models;

public record BoardInfo(
    string Id,
    string Name,
    string WorkspaceId,
    string? WorkspaceName,
    bool IsFavorite
);
