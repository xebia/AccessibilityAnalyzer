using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AccessibilityAnalyzer.Ai.Steps;

public class
    KeyboardNavigationAnalyzerStep : KernelProcessStep<KeyboardNavigationAnalyzerStep.KeyboardNavigationAnalyzerState>
{
    private const string SystemPrompt = $$$"""
                                           You are an accessibility expert. Your role is to analyze HTML code.
                                           Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:
                                            
                                           Keyboard-Specific Accessibility Checks
                                           1. Focusable Elements
                                              - Identify interactive elements (buttons, links, inputs) that are not keyboard-focusable.
                                              - Ensure that all form fields and controls are accessible via `Tab`.
                                              - Detect clickable divs/spans that lack `tabindex='0'`, making them inaccessible.
                                            
                                           2️. Tab Order & Logical Navigation
                                              - Verify that tab navigation follows a logical reading sequence based on the HTML_CODE language.
                                              - Identify incorrectly used `tabindex` values (avoid `tabindex > 0`).
                                              - Detect elements causing focus jumps or loss after interaction.
                                            
                                           3️. Keyboard Traps & Modal Issues
                                              - Identify elements where keyboard navigation gets stuck (users cannot navigate past certain sections).
                                              - Ensure modal dialogs, popups, and overlays can be dismissed using `Esc` or `Tab`.
                                              - Check if the focus is trapped inside a modal or dropdown menu.
                                            
                                           4️. Skip Links
                                              - Ensure the presence of a 'Skip to Content' link for better keyboard navigation.

                                           {{{Constants.OutputFormat}}}
                                           """;

    private KeyboardNavigationAnalyzerState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<KeyboardNavigationAnalyzerState> state)
    {
        _state = state.State!;
        _state.ChatHistory ??= new ChatHistory(SystemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AnalyzeKeyboardNavigation)]
    public async Task AnalyzeKeyboardNavigation(Kernel kernel, KernelProcessStepContext context, string htmlContent)
    {
        Console.WriteLine($"{nameof(AltTextAnalyzerStep)}:\n\tAnalyzing keyboard navigation...");

        // Add the new product info to the chat history
        _state.ChatHistory!.AddUserMessage($"HTML_CODE:\n\n{htmlContent}");

        // Get a response from the LLM
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse =
            await chatCompletionService.GetChatMessageContentAsync(_state.ChatHistory!);

        await context.EmitEventAsync(OutputEvents.KeyboardNavigationAnalyzed, generatedDocumentationResponse.Content!);
    }

    public static class Functions
    {
        public const string AnalyzeKeyboardNavigation = nameof(AnalyzeKeyboardNavigation);
    }

    public static class OutputEvents
    {
        public const string KeyboardNavigationAnalyzed = nameof(KeyboardNavigationAnalyzed);
    }

    public class KeyboardNavigationAnalyzerState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}