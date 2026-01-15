---
name: using-enify
description: Manage Enify workspaces and boards.
Use when user asks to:
- List, find, or show workspaces
- List, find, or show boards
- Start, stop, or refresh a board
- Restore displays
- Check Enify status
---

# Using Enify

Control Enify workspaces and boards through Claude. Enify is a window management application that saves and restores window layouts called "boards".

## Available Tools

| Tool | Description |
|------|-------------|
| `list_workspaces` | List all Enify workspaces |
| `list_boards` | List all boards (filter by workspace or favorites) |
| `get_recent_boards` | Get recently used boards |
| `start_board` | Start a board by its ID |
| `stop_board` | Stop the currently running board |
| `refresh_board` | Refresh the current board's window arrangement |
| `restore_displays` | Restore display/monitor configuration |

## Available Resources

Browse Enify data directly:

| Resource URI | Description |
|--------------|-------------|
| `enify://workspaces` | All workspaces |
| `enify://boards` | All boards |
| `enify://boards/favorites` | Favorite boards only |
| `enify://boards/recent` | Recently used boards |
| `enify://status` | Enify installation status |

## Available Prompts

Pre-defined workflows:

| Prompt | Description |
|--------|-------------|
| `browse_workspaces` | Explore workspaces and their boards |
| `start_board_workflow` | Step-by-step guide to start a board |
| `manage_current_board` | Options for the running board |
| `quick_start_favorites` | Quickly start a favorite board |
| `enify_status_check` | Verify Enify is working |

## Workflows

### Quick Start a Board
1. "List my workspaces" - See available workspaces
2. "Show boards in Development" - Filter by workspace
3. "Start board VG News" - Launch the board

### Start a Favorite Board
1. "Show my favorite boards" - List favorites only
2. "Start the first one" - Launch selected board

### Manage Current Board
- "Refresh the board" - Re-apply window layout
- "Stop the board" - Close board and restore previous layout
- "Restore displays" - Fix monitor configuration

## Example Commands

- "Show my Enify workspaces"
- "List all boards"
- "Show only favorite boards"
- "Start the VG board"
- "Stop the current board"
- "Refresh the board"
- "Restore my displays"
- "Check if Enify is working"

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Enify not installed" | Ensure Enify is installed and in PATH |
| Board doesn't start | Check board ID with `list_boards` first |
| Windows not arranged | Try `refresh_board` to re-apply layout |
| Display issues | Use `restore_displays` to fix monitor config |
