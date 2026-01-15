using System.ComponentModel;
using ModelContextProtocol.Server;

namespace EnifyMcp.Prompts;

[McpServerPromptType]
public class EnifyPrompts
{
    [McpServerPrompt(Name = "browse_workspaces")]
    [Description("Guide to explore Enify workspaces and their boards")]
    public string BrowseWorkspaces()
    {
        return """
            Help me explore my Enify workspaces:
            1. First, list all my workspaces using list_workspaces
            2. Then show me the boards in each workspace
            3. Highlight which boards are marked as favorites
            4. Summarize how my workspaces are organized
            """;
    }

    [McpServerPrompt(Name = "start_board_workflow")]
    [Description("Step-by-step guide to find and start an Enify board")]
    public string StartBoardWorkflow()
    {
        return """
            Help me start an Enify board:
            1. First, list my available boards using list_boards
            2. Ask me which board I want to start (show me the options)
            3. Once I choose, start the board using start_board with the board ID
            4. Confirm the board has started successfully
            """;
    }

    [McpServerPrompt(Name = "manage_current_board")]
    [Description("Options for managing the currently running board")]
    public string ManageCurrentBoard()
    {
        return """
            Help me manage my current Enify board. I can:
            - Refresh the board layout (if windows have moved): use refresh_board
            - Stop the current board entirely: use stop_board
            - Restore my display configuration: use restore_displays

            Ask me what I'd like to do with my current board setup.
            """;
    }

    [McpServerPrompt(Name = "quick_start_favorites")]
    [Description("Quickly start one of your favorite boards")]
    public string QuickStartFavorites()
    {
        return """
            Show me my favorite Enify boards and help me start one:
            1. List only my favorite boards using list_boards with favoritesOnly=true
            2. Present them in a numbered list
            3. Let me pick one by number or name
            4. Start the selected board
            """;
    }

    [McpServerPrompt(Name = "enify_status_check")]
    [Description("Check if Enify is properly installed and working")]
    public string StatusCheck()
    {
        return """
            Check my Enify setup:
            1. Verify Enify is installed by trying to list workspaces
            2. If it works, show a summary of:
               - Number of workspaces
               - Total number of boards
               - Number of favorite boards
            3. If it fails, explain what might be wrong
            """;
    }
}
