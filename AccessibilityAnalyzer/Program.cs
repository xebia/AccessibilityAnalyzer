using AccessibilityAnalyzer;
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

builder.Services.AddOpenAiServices();

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
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return Results.BadRequest<string>("Invalid URL");
            }

            var analyzeUrlResult = await analyzer.AnalyzeUrl(uri);
            return Results.Ok(analyzeUrlResult);
        })
    .WithName("AnalyzeUrl");

app.Run();