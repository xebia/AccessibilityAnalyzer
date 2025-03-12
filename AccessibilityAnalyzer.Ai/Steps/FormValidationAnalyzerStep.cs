using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AccessibilityAnalyzer.Ai.Steps;

public class FormValidationAnalyzerStep : KernelProcessStep<FormValidationAnalyzerStep.FormValidationAnalyzerState>
{
    private const string SystemPrompt = $$$"""
                                           You are an accessibility expert. Your role is to analyze HTML code.
                                           Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                                           Form and input specific Accessibility Checks
                                           1. Input Type Validation  
                                           - Ensure inputs use correct types (`tel`, `email`, `password`, `date`, `number`). Use the context to determine the correct type.

                                           2. Label & Association
                                           - Check if every input field has an associated `<label>` or `aria-label`.
                                           - Ensure labels are programmatically linked using `for` and `id` attributes if the field not wrapped in a label.

                                           3. Required Fields & Validation
                                           - Ensure required fields use the `required` attribute.
                                           - Check if error messages are linked using `aria-describedby`.
                                           - Detect fields that rely only on placeholder text for labels (bad practice).

                                           {{{Constants.OutputFormat}}}
                                           """;

    private FormValidationAnalyzerState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<FormValidationAnalyzerState> state)
    {
        _state = state.State!;
        _state.ChatHistory ??= new ChatHistory(SystemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AnalyzeFormValidation)]
    public async Task AnalyzeFormValidation(Kernel kernel, KernelProcessStepContext context, string htmlContent)
    {
        Console.WriteLine($"{nameof(FormValidationAnalyzerStep)}:\n\tAnalyzing form validation...");

        // Add the new product info to the chat history
        _state.ChatHistory!.AddUserMessage($"HTML_CODE:\n\n{htmlContent}");

        // Get a response from the LLM
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse =
            await chatCompletionService.GetChatMessageContentAsync(_state.ChatHistory!);

        await context.EmitEventAsync(OutputEvents.FormValidationAnalyzed, generatedDocumentationResponse.Content!);
    }

    public static class Functions
    {
        public const string AnalyzeFormValidation = nameof(AnalyzeFormValidation);
    }

    public static class OutputEvents
    {
        public const string FormValidationAnalyzed = nameof(FormValidationAnalyzed);
    }

    public class FormValidationAnalyzerState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}