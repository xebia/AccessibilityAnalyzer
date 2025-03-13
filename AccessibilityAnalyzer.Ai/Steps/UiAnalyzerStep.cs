using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.ImageToText;

namespace AccessibilityAnalyzer.Ai.Steps;

public class UiAnalyzerStep : KernelProcessStep<UiAnalyzerStep.UiAnalyzerState>
{
    private UiAnalyzerState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<UiAnalyzerState> state)
    {
        _state = state.State!;
        _state.ChatHistory ??= new ChatHistory();

        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AnalyzeUi)]
    public async Task AnalyzeUi(Kernel kernel, KernelProcessStepContext context, byte[] imageBytes)
    {
        Console.WriteLine($"{nameof(UiAnalyzerStep)}:\n\tAnalyzing UI accessibility...");

        var imageContent = new ImageContent(imageBytes, Constants.ImageMimeType);

        
        // Get a response from the LLM
        var imageToTextService = kernel.GetRequiredService<IImageToTextService>();
        var uiDataResponse = await imageToTextService.GetTextContentsAsync(imageContent);

        await context.EmitEventAsync(OutputEvents.UiAnalyzed, uiDataResponse);
    }

    public static class Functions
    {
        public const string AnalyzeUi = nameof(AnalyzeUi);
    }

    public static class OutputEvents
    {
        public const string UiAnalyzed = nameof(UiAnalyzed);
    }

    public class UiAnalyzerState
    {
        public ChatHistory? ChatHistory { get; set; }
    }
}