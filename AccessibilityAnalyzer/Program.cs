using AccessibilityAnalyzer;
using AccessibilityAnalyzer.SourceGathering;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<SourceGathering>();
builder.Services.AddScoped<IAnalyzer, Analyzer>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/analyze",
        (
            [FromServices] IAnalyzer analyzer,
            [FromQuery(Name = "url")] string url
        ) =>
        {
            return analyzer.AnalyzeUrl(url);
        })
    .WithName("AnalyzeUrl");

app.Run();