using System.Text.Json;
using AccessibilityAnalyzer.Ai.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AccessibilityAnalyzer.Ai.Steps.Ui;

public class UiFontsAnalysisStep : KernelProcessStep<UiFontsAnalysisStep.UiFontsAnalysisState>
{
    private const string SystemPrompt = $$$"""
                                           You are an accessibility expert. Your role is to analyze a website's screenshot for accessibility issues.
                                           Alongside the image you will be provided by a list of elements ELEMENT_LIST, their bounding boxes, and their text content.
                                           Corelating the image with the elements, you will need to
                                           Analyze the image for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                                           Typography and Font Usage
                                           1. Font Size and Readability
                                           - Ensure text is sufficiently large (minimum 16px for body text recommended).
                                           - Verify that fonts are readable and legible across different elements.
                                           
                                           2. Font Styling
                                           - Identify if any decorative or cursive fonts are used in ways that reduce readability.
                                           - Check if font weight (boldness) is used appropriately to emphasize important content.
                                           - Verify line height (spacing between lines) is at least 1.5 times the font size for body text.
                                           
                                           3. Text Layout and Structure
                                           - Assess if letter spacing (tracking) and word spacing allow for easy reading.
                                           - Check that line length is appropriate (typically 50-75 characters per line).
                                           - Verify text is properly aligned (left-aligned is generally most readable) and justified text doesn't create readability issues.

                                           Note: Since only a screenshot is provided, ensure to carefully analyze content visually and through any provided metadata (ELEMENT_LIST) for deficiencies.

                                           {{{Constants.OutputFormat}}}
                                           """;

    private UiFontsAnalysisState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<UiFontsAnalysisState> state)
    {
        _state = state.State!;
        _state.ChatHistory ??= new ChatHistory(SystemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AnalyzeUiFonts)]
    public async Task AnalyzeUiFonts(Kernel kernel, KernelProcessStepContext context, UiAnalysisData uiAnalysisData)
    {
        _state.ChatHistory!.AddUserMessage([
            new TextContent($"ELEMENT_LIST:\n\n{JsonSerializer.Serialize(uiAnalysisData.ElementList)}"),
            new ImageContent(uiAnalysisData.Screenshot, Constants.ImageMimeType)
        ]);

        // Get a response from the LLM
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>(Constants.Gpt4ServiceKey);
        var generatedDocumentationResponse =
            await chatCompletionService.GetChatMessageContentAsync(_state.ChatHistory!);

        await context.EmitEventAsync(OutputEvents.UiFontsAnalyzed, generatedDocumentationResponse.Content!);
    }

    public static class Functions
    {
        public const string AnalyzeUiFonts = nameof(AnalyzeUiFonts);
    }

    public static class OutputEvents
    {
        public const string UiFontsAnalyzed = nameof(UiFontsAnalyzed);
    }

    public class UiFontsAnalysisState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}
