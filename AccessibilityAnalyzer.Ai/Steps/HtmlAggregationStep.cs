using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai.Steps;

public class HtmlAggregationStep : KernelProcessStep<HtmlAggregationStep.HtmlAggregateState>
{
    private HtmlAggregateState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<HtmlAggregateState> state)
    {
        _state = state.State!;
        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.AltTextWasAnalyzed)]
    public async Task AltTextWasAnalyzed(Kernel kernel, KernelProcessStepContext context, string response)
    {
        _state.AltTextAnalysisResponse = CleanResponse(response);
        _state.AltTextAnalyzed = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }
    
    [KernelFunction(Functions.KeyboardNavigationWasAnalyzed)]
    public async Task KeyboardNavigationWasAnalyzed(Kernel kernel, KernelProcessStepContext context, string response)
    {
        _state.KeyboardNavigationAnalysisResponse = CleanResponse(response);
        _state.KeyboardNavigationAnalyzed = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }

    [KernelFunction(Functions.FormValidationWasAnalyzed)]
    public async Task FormValidationWasAnalyzed(Kernel kernel, KernelProcessStepContext context, string response)
    {
        _state.FormValidationAnalysisResponse = CleanResponse(response);
        _state.FormValidationAnalyzed = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }
    
    [KernelFunction(Functions.SemanticStructureWasAnalyzed)]
    public async Task SemanticStructureWasAnalyzed(Kernel kernel, KernelProcessStepContext context, string response)
    {
        _state.SemanticStructureAnalysisResponse = CleanResponse(response);
        _state.SemanticStructureAnalyzed = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }

    private async Task CheckIfAnalysisIsCompleteAsync(KernelProcessStepContext context)
    {
        if (_state.AltTextAnalyzed && 
            _state.KeyboardNavigationAnalyzed &&
            _state.FormValidationAnalyzed &&
            _state.SemanticStructureAnalyzed)
            await context.EmitEventAsync(OutputEvents.AnalysisComplete, AggregateResults());
    }

    private string CleanResponse(string response)
    {
        const string markdownOpen = "```json";
        const string markdownClose = "```";
        
        return response.Replace(markdownOpen, "").Replace(markdownClose, "").Trim();
    }
    
    private string AggregateResults()
    {
        return $"[{
            string.Join(",", 
                _state.AltTextAnalysisResponse, 
                _state.KeyboardNavigationAnalysisResponse,
                _state.FormValidationAnalysisResponse,
                _state.SemanticStructureAnalysisResponse
            )
        }]"; 
    }
    
    public static class Functions
    {
        public const string AltTextWasAnalyzed = nameof(AltTextWasAnalyzed);
        public const string KeyboardNavigationWasAnalyzed = nameof(KeyboardNavigationWasAnalyzed);
        public const string FormValidationWasAnalyzed = nameof(FormValidationWasAnalyzed);
        public const string SemanticStructureWasAnalyzed = nameof(SemanticStructureWasAnalyzed);
    }

    public static class OutputEvents
    {
        public const string AnalysisComplete = nameof(AnalysisComplete);
    }

    public class HtmlAggregateState
    {
        public bool AltTextAnalyzed { get; set; }
        public string AltTextAnalysisResponse { get; set; }
        public bool KeyboardNavigationAnalyzed { get; set; }
        public string KeyboardNavigationAnalysisResponse { get; set; }
        public bool FormValidationAnalyzed { get; set; }
        public string FormValidationAnalysisResponse { get; set; }
        public bool SemanticStructureAnalyzed { get; set; }
        public string SemanticStructureAnalysisResponse { get; set; }
    }
}