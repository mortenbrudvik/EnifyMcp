namespace EnifyMcp.Core.Models.Results;

public record OperationResult(
    bool Success,
    string? Error = null,
    string? ErrorCode = null
);
