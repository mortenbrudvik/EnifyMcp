namespace EnifyMcp.Core.Models.Results;

public record BoardDto(
    string Id,
    string Name,
    string WorkspaceId,
    string? WorkspaceName,
    bool IsFavorite
);
