using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AccessibilityAnalyzer.Ai.Steps;

public class SemanticCodeAnalyzerStep : KernelProcessStep<SemanticCodeAnalyzerStep.SemanticCodeAnalyzerState>
{
    private const string SystemPrompt = $$$"""
                                           You are an accessibility expert. Your role is to analyze HTML code.
                                           Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                                           Semantically correct HTML
                                           1. Proper Use of Semantic Elements
                                           - Verify that elements are used appropriately for their intended purpose (e.g., <header>, <nav>, <section>, <article>, <footer>, <aside>).
                                           - Identify misused or missing structural elements, such as content being wrapped in <div> or <span> instead of proper semantic tags.
                                           - Ensure headings (<h1> to <h6>) are used in a meaningful and hierarchical order that reflects the document’s structure.

                                           2. Landmark Roles and Navigability
                                           - Confirm that key page areas (e.g., banner, navigation, main content, complementary content, footer) use appropriate ARIA landmarks or native semantic tags.
                                           - Detect redundant use of ARIA roles when native semantic elements (e.g., <nav>, <main>, <aside>) already provide the same accessibility information.

                                           3. Overall Hierarchy and Readability
                                           - Assess whether the HTML document follows a logical hierarchy and structure that is meaningful and accessible to both users and assistive technologies.
                                           - Verify proper nesting of elements to avoid invalid or broken HTML (e.g., ensuring no block elements are placed inside inline elements).

                                           {{{Constants.OutputFormat}}}
                                           """;

    private SemanticCodeAnalyzerState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<SemanticCodeAnalyzerState> state)
    {
        _state = state.State!;
        _state.ChatHistory ??= new ChatHistory(SystemPrompt);

        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AnalyzeSemanticStructure)]
    public async Task AnalyzeSemanticStructure(Kernel kernel, KernelProcessStepContext context, string htmlContent)
    {
        Console.WriteLine($"{nameof(SemanticCodeAnalyzerStep)}:\n\tAnalyzing semantic structure...");

        // Add the new product info to the chat history
        _state.ChatHistory!.AddUserMessage($"HTML_CODE:\n\n{htmlContent}");

        // Get a response from the LLM
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        var generatedDocumentationResponse =
            await chatCompletionService.GetChatMessageContentAsync(_state.ChatHistory!);

        await context.EmitEventAsync(OutputEvents.SemanticStructureAnalyzed, generatedDocumentationResponse.Content!);
    }

    public static class Functions
    {
        public const string AnalyzeSemanticStructure = nameof(AnalyzeSemanticStructure);
    }

    public static class OutputEvents
    {
        public const string SemanticStructureAnalyzed = nameof(SemanticStructureAnalyzed);
    }

    public class SemanticCodeAnalyzerState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}