using System.Text.Json;
using AccessibilityAnalyzer.Ai.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AccessibilityAnalyzer.Ai.Steps;

public class UiColorAnalysisStep : KernelProcessStep<UiColorAnalysisStep.UiColorAnalysisState>
{
    private const string SystemPrompt = $$$"""
                                           You are an accessibility expert. Your role is to analyze a website's screenshot for accessibility issues.
                                           Alongside the image you will be provided by a list of elements ELEMENT_LIST, their bounding boxes, and their text content.
                                           Corelating the image with the elements, you will need to
                                           Analyze the image for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                                           Colors being used
                                           1. Color Contrast
                                           - Ensure all text has enough color contrast ratio against its background (minimum 4.5:1 for normal text and 3:1 for large text, per WCAG 2.1).
                                           - Verify if interactive components (like buttons or links) also meet the color contrast requirements.
                                           - Check contrast for non-text elements conveying information (icons, graphical objects, etc.).

                                           2. Color Dependence
                                           - Identify if any content relies only on color to convey meaning (e.g., error messages marked only in red). Ensure additional visual or contextual cues are provided.
                                           - Look for interactive elements (like links or buttons) and ensure they are distinguishable without relying solely on color.

                                           3. Focus States
                                           - If visible, verify that focus indicators for interactive elements meet contrast standards and are visually distinguishable.

                                           Note: Since only a screenshot is provided, ensure to carefully analyze content visually and through any provided metadata (ELEMENT_LIST) for deficiencies.

                                           {{{Constants.OutputFormat}}}
                                           """;

    private UiColorAnalysisState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<UiColorAnalysisState> state)
    {
        _state = state.State!;
        _state.ChatHistory ??= new ChatHistory(SystemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AnalyzeUiColor)]
    public async Task AnalyzeUiColor(Kernel kernel, KernelProcessStepContext context, UiAnalysisData uiAnalysisData)
    {
        _state.ChatHistory!.AddUserMessage([
            new TextContent($"ELEMENT_LIST:\n\n{JsonSerializer.Serialize(uiAnalysisData.ElementList)}"),
            new ImageContent(uiAnalysisData.Screenshot, Constants.ImageMimeType)
        ]);

        // Get a response from the LLM
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse =
            await chatCompletionService.GetChatMessageContentAsync(_state.ChatHistory!);

        await context.EmitEventAsync(OutputEvents.UiColorAnalyzed, generatedDocumentationResponse.Content!);
    }

    public static class Functions
    {
        public const string AnalyzeUiColor = nameof(AnalyzeUiColor);
    }

    public static class OutputEvents
    {
        public const string UiColorAnalyzed = nameof(UiColorAnalyzed);
    }

    public class UiColorAnalysisState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}