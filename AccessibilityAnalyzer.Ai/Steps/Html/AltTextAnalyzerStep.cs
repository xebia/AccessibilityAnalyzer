using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AccessibilityAnalyzer.Ai.Steps.Html;

public class AltTextAnalyzerStep : KernelProcessStep<AltTextAnalyzerStep.AltTextAnalyzerState>
{
    private const string SystemPrompt = $$$"""
                                           You are an accessibility expert. Your role is to analyze HTML code.
                                           Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                                           Alt Text for Images:
                                           1. Meaningful Images Without Alt Text
                                              - Identify images that convey meaningful content but do not have alt text.
                                              - Ensure that all images with a functional or informational purpose have descriptive alt text.

                                           2. Decorative Images with Alt Text
                                              - Check that purely decorative images (such as those styled with CSS or providing no information) do not have alt text or have an empty alt="" attribute.
                                              - Make sure that images used for visual styling (e.g., icons, borders) do not have misleading alt text.

                                           3. Properly Descriptive Alt Text
                                              - Check that alt text is concise, clear, and provides context, especially for images that convey critical information (graphs, charts, buttons).
                                              - Ensure long or complex images have alternative descriptions, possibly linking to a more detailed description elsewhere.
                                              - Alt text cannot be empty for meaningful images, just alt or alt="" is not correct
                                              
                                           {{{Constants.OutputFormat}}}
                                           """;

    private AltTextAnalyzerState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<AltTextAnalyzerState> state)
    {
        _state = state.State!;
        _state.ChatHistory ??= new ChatHistory(SystemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AnalyzeAltText)]
    public async Task AnalyzeAltText(Kernel kernel, KernelProcessStepContext context, string htmlContent)
    {
        Console.WriteLine($"{nameof(AltTextAnalyzerStep)}:\n\tAnalyzing alt texts...");

        // Add the new product info to the chat history
        _state.ChatHistory!.AddUserMessage($"HTML_CODE:\n\n{htmlContent}");

        // Get a response from the LLM
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>(Constants.Gpt3ServiceKey);
        var generatedDocumentationResponse =
            await chatCompletionService.GetChatMessageContentAsync(_state.ChatHistory!);

        await context.EmitEventAsync(OutputEvents.AltTextAnalyzed, generatedDocumentationResponse.Content!);
    }

    public static class Functions
    {
        public const string AnalyzeAltText = nameof(AnalyzeAltText);
    }

    public static class OutputEvents
    {
        public const string AltTextAnalyzed = nameof(AltTextAnalyzed);
    }


    public class AltTextAnalyzerState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}