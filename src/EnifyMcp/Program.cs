using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using EnifyMcp.Core.Services;
using EnifyMcp.Tools;

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

// Register MCP tools
builder.Services.AddSingleton<WorkspaceTools>();
builder.Services.AddSingleton<BoardTools>();
builder.Services.AddSingleton<DisplayTools>();

// Configure MCP Server
builder.Services.AddMcpServer(options =>
{
    options.ServerInfo = new()
    {
        Name = "enify",
        Version = "1.0.0"
    };
})
.WithStdioServerTransport()
.WithTools<WorkspaceTools>()
.WithTools<BoardTools>()
.WithTools<DisplayTools>();

var app = builder.Build();

await app.RunAsync();
