namespace EnifyMcp.Core.Models;

public enum EnifyErrorCode
{
    None = 0,
    EnifyNotInstalled = 1,
    CommandTimeout = 2,
    InvalidBoardId = 3,
    CommandFailed = 4,
    JsonParseError = 5,
    NoBoardRunning = 6
}
