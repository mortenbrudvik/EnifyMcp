using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using EnifyMcp.Core.Services;
using EnifyMcp.Tools;
using EnifyMcp.Resources;
using EnifyMcp.Prompts;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging to stderr (important for STDIO-based MCP servers)
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});
builder.Logging.SetMinimumLevel(LogLevel.Warning);

// Register services
builder.Services.AddSingleton<IEnifyService, EnifyService>();

// Register MCP tools, resources, and prompts
builder.Services.AddSingleton<WorkspaceTools>();
builder.Services.AddSingleton<BoardTools>();
builder.Services.AddSingleton<DisplayTools>();
builder.Services.AddSingleton<EnifyResources>();
builder.Services.AddSingleton<EnifyPrompts>();

// Configure MCP Server
builder.Services.AddMcpServer(options =>
{
    options.ServerInfo = new()
    {
        Name = "enify",
        Version = "1.1.0"
    };
})
.WithStdioServerTransport()
.WithTools<WorkspaceTools>()
.WithTools<BoardTools>()
.WithTools<DisplayTools>()
.WithResources<EnifyResources>()
.WithPrompts<EnifyPrompts>();

var app = builder.Build();

await app.RunAsync();
