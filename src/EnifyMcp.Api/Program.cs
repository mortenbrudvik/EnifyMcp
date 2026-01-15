using EnifyMcp.Core.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure port
builder.WebHost.UseUrls("http://localhost:5050");

// Add services
builder.Services.AddSingleton<IEnifyService, EnifyService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Enify API",
        Version = "v1",
        Description = "REST API for controlling Enify workspaces and boards. Compatible with ChatGPT GPT Actions.",
        Contact = new OpenApiContact
        {
            Name = "Enify API"
        }
    });
});

// Add CORS for ChatGPT
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowChatGPT", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Enify API v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseCors("AllowChatGPT");
app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () => new { Status = "healthy", Timestamp = DateTime.UtcNow });

app.Run();
