using LogAnalysis.Core.Interfaces;
using LogAnalysis.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://+:5001");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Log Analysis API", Version = "v1" });
});
builder.Services.AddSingleton<ILogAnalysisService, LogAnalysisService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Log Analysis API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors();

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
    .WithName("Health")
    .WithOpenApi(operation => new(operation)
    {
        Summary = "Health Check",
        Description = "Returns the health status of the API"
    });

app.MapGet("/api/loganalysis", async (ILogAnalysisService logAnalysisService) =>
{
    var logPath = Path.Combine("/app/logs", "webrtc_studio.log");
    try
    {
        var result = await logAnalysisService.AnalyzeLogFileAsync(logPath);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Error analyzing log file",
            detail: ex.Message,
            statusCode: 500
        );
    }
})
.WithName("GetLogAnalysis")
.WithOpenApi(operation => new(operation)
{
    Summary = "Analyze Log File",
    Description = "Analyzes the WebRTC studio log file and returns user activity and error statistics",
    Tags = new[] { new Microsoft.OpenApi.Models.OpenApiTag { Name = "Log Analysis" } }
});

app.Run();
