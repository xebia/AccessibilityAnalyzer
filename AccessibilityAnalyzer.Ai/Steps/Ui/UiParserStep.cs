using System.Text.Json;
using AccessibilityAnalyzer.Ai.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;

namespace AccessibilityAnalyzer.Ai.Steps.Ui;

public class UiParserStep : KernelProcessStep
{
    [KernelFunction(Functions.ParseUi)]
    public async Task ParseUi(Kernel kernel, KernelProcessStepContext context, PageData pageData)
    {
        var imageContent = new ImageContent(pageData.Screenshot, Constants.ImageMimeType);

        // Get a response from the LLM
        var imageToTextService = kernel.GetRequiredService<IImageToTextService>();
        var uiDataResponse = await imageToTextService.GetTextContentsAsync(imageContent);

        var response = JsonSerializer.Deserialize<OmniParserResult>(uiDataResponse[0].Text!, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await context.EmitEventAsync(OutputEvents.UiParsed,
            new UiAnalysisData(response!.ParsedContentList, pageData.Screenshot));
    }

    public static class Functions
    {
        public const string ParseUi = nameof(ParseUi);
    }

    public static class OutputEvents
    {
        public const string UiParsed = nameof(UiParsed);
    }
}