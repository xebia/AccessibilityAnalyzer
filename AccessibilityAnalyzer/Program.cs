using AccessibilityAnalyzer;
using AccessibilityAnalyzer.Dto;
using AccessibilityAnalyzer.Extensions;
using AccessibilityAnalyzer.SourceGathering;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
builder.Services.AddScoped<SourceGathering>();
builder.Services.AddScoped<IAnalyzer, Analyzer>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

builder.Services.AddOpenAiServices();
builder.Services.AddAiAnalysis();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/analyze",
        async (
            [FromServices] IAnalyzer analyzer,
            [FromQuery(Name = "url")] string url
        ) =>
        {
            if (string.IsNullOrEmpty(url)) return Results.BadRequest("URL is required");

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
                return Results.BadRequest("Invalid URL. Please provide a valid absolute URL (http or https)");

            var analyzeUrlResult = await analyzer.AnalyzeUrl(uri);

            if (analyzeUrlResult == null)
            {
                return Results.InternalServerError("Not able to analyze the website");
            }
            
            // Convert to AccessibilityAnalysis objects
            var accessibilityAnalyses = analyzeUrlResult.Select(AccessibilityAnalysis.FromModel).ToArray();
            
            // Combine all analyses into a single object
            var combinedAnalysis = new AccessibilityAnalysis
            {
                Analysis = accessibilityAnalyses.SelectMany(a => a.Analysis).ToList(),
                Summary = new Summary
                {
                    Failed = accessibilityAnalyses.Sum(a => a.Summary?.Failed ?? 0)
                }
            };
            
            return Results.Ok(combinedAnalysis);
        })
    .WithName("AnalyzeUrl");

app.Run();