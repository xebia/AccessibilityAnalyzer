using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai.Steps;

public class VisualAggregationStep : KernelProcessStep<VisualAggregationStep.VisualAggregateState>
{
    private VisualAggregateState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<VisualAggregateState> state)
    {
        _state = state.State!;
        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.UiColorWasAnalyzed)]
    public async Task UiColorWasAnalyzed(Kernel kernel, KernelProcessStepContext context, string response)
    {
        _state.UiColorAnalysisResponse = CleanResponse(response);
        _state.UiColorAnalyzed = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }
    
    [KernelFunction(Functions.UiFontsWereAnalyzed)]
    public async Task UiFontsWereAnalyzed(Kernel kernel, KernelProcessStepContext context, string response)
    {
        _state.UiFontsAnalysisResponse = CleanResponse(response);
        _state.UiFontsAnalyzed = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }

    private async Task CheckIfAnalysisIsCompleteAsync(KernelProcessStepContext context)
    {
        if (_state.UiColorAnalyzed && _state.UiFontsAnalyzed)
            await context.EmitEventAsync(OutputEvents.VisualAnalysisComplete, AggregateResults());
    }

    private string CleanResponse(string response)
    {
        const string markdownOpen = "```json";
        const string markdownClose = "```";
        
        return response.Replace(markdownOpen, "").Replace(markdownClose, "").Trim();
    }
    
    private string AggregateResults()
    {
        return $"[{string.Join(",", _state.UiColorAnalysisResponse, _state.UiFontsAnalysisResponse)}]"; 
    }
    
    public static class Functions
    {
        public const string UiColorWasAnalyzed = nameof(UiColorWasAnalyzed);
        public const string UiFontsWereAnalyzed = nameof(UiFontsWereAnalyzed);
    }

    public static class OutputEvents
    {
        public const string VisualAnalysisComplete = nameof(VisualAnalysisComplete);
    }

    public class VisualAggregateState
    {
        public bool UiColorAnalyzed { get; set; }
        public string UiColorAnalysisResponse { get; set; } = string.Empty;
        public bool UiFontsAnalyzed { get; set; }
        public string UiFontsAnalysisResponse { get; set; } = string.Empty;
    }
}
