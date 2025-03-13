using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai.Steps;

public class AnalysisAggregationStep : KernelProcessStep<AnalysisAggregationStep.AnalysisAggregateState>
{
    private AnalysisAggregateState _state = new();

    public override ValueTask ActivateAsync(KernelProcessStepState<AnalysisAggregateState> state)
    {
        _state = state.State!;
        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.HtmlAnalysisReady)]
    public async Task HtmlAnalysisReady(Kernel kernel, KernelProcessStepContext context, object response)
    {
        _state.HtmlAnalysisResponse = response;
        _state.HtmlAnalysisComplete = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }

    [KernelFunction(Functions.VisualAnalysisReady)]
    public async Task VisualAnalysisReady(Kernel kernel, KernelProcessStepContext context, object response)
    {
        _state.VisualAnalysisResponse = response;
        _state.VisualAnalysisComplete = true;
        await CheckIfAnalysisIsCompleteAsync(context);
    }

    private async Task CheckIfAnalysisIsCompleteAsync(KernelProcessStepContext context)
    {
        if (_state.HtmlAnalysisComplete && _state.VisualAnalysisComplete)
            await context.EmitEventAsync(OutputEvents.AnalysisComplete, AggregateResults());
    }

    private object AggregateResults()
    {
        // For now just returning an array of both analysis results
        return new[] { _state.HtmlAnalysisResponse, _state.VisualAnalysisResponse };
    }

    public static class Functions
    {
        public const string HtmlAnalysisReady = nameof(HtmlAnalysisReady);
        public const string VisualAnalysisReady = nameof(VisualAnalysisReady);
    }

    public static class OutputEvents
    {
        public const string AnalysisComplete = nameof(AnalysisComplete);
    }

    public class AnalysisAggregateState
    {
        public bool HtmlAnalysisComplete { get; set; }
        public object HtmlAnalysisResponse { get; set; }
        public bool VisualAnalysisComplete { get; set; }
        public object VisualAnalysisResponse { get; set; }
    }
}