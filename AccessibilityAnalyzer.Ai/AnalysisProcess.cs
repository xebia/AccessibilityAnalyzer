using AccessibilityAnalyzer.Ai.Models;
using AccessibilityAnalyzer.Ai.Processes;
using AccessibilityAnalyzer.Ai.Steps;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process.Tools;

namespace AccessibilityAnalyzer.Ai;

public interface IAnalysisProcess
{
    Task<AccessibilityAnalysis[]?> StartProcess(string htmlContent, byte[] screenshot);
}

public class AnalysisProcess(Kernel kernel) : IAnalysisProcess
{
    public async Task<AccessibilityAnalysis[]?> StartProcess(string htmlContent, byte[] screenshot)
    {
        var processBuilder = new ProcessBuilder(nameof(AnalysisProcess));

        var htmlAnalysisStep = processBuilder.AddStepFromProcess(HtmlAnalysisProcess.CreateProcess());
        var visualAnalysisStep = processBuilder.AddStepFromProcess(VisualAnalysisProcess.CreateProcess());

        // todo: add correct aggregation step
        var analysisAggregationStep = processBuilder.AddStepFromType<AnalysisAggregationStep>();


        processBuilder
            .OnInputEvent(ProcessEvents.Start)
            .SendEventTo(
                htmlAnalysisStep.WhereInputEventIs(HtmlAnalysisProcess.ProcessEvents.StartHtmlContentAnalysis))
            .SendEventTo(visualAnalysisStep.WhereInputEventIs(VisualAnalysisProcess.ProcessEvents.StartVisualAnalysis));

        htmlAnalysisStep
            .OnEvent(HtmlAnalysisProcess.ProcessEvents.HtmlContentAnalysisReady)
            .SendEventTo(new ProcessFunctionTargetBuilder(analysisAggregationStep,
                AnalysisAggregationStep.Functions.HtmlAnalysisReady));

        visualAnalysisStep
            .OnEvent(VisualAnalysisProcess.ProcessEvents.VisualAnalysisReady)
            .SendEventTo(new ProcessFunctionTargetBuilder(analysisAggregationStep,
                AnalysisAggregationStep.Functions.VisualAnalysisReady));

        analysisAggregationStep
            .OnEvent(AnalysisAggregationStep.OutputEvents.AnalysisComplete);

        var process = processBuilder.Build();

        var mermaid = processBuilder.ToMermaid();

        var processContext =
            await process.StartAsync(kernel,
                new KernelProcessEvent { Id = ProcessEvents.Start, Data = new PageData(htmlContent, screenshot) });

        // get the data out of the last step
        var processInfo = await processContext.GetStateAsync();
        var step = processInfo.Steps.FirstOrDefault(s => s.State.Name == nameof(AnalysisAggregationStep));
        if (step == null) return null;

        var state = step.State as KernelProcessStepState<AnalysisAggregationStep.AnalysisAggregateState>;
        return state!.State!.AggregateResults();
    }

    public struct ProcessEvents
    {
        public const string Start = nameof(Start);
    }
}
/*
flowchart LR
   Start["Start"]
   End["End"]
   AltTextAnalyzerStep["AltTextAnalyzerStep"]
   AltTextAnalyzerStep["AltTextAnalyzerStep"] --> HtmlAggregationStep["HtmlAggregationStep"]
   KeyboardNavigationAnalyzerStep["KeyboardNavigationAnalyzerStep"]
   KeyboardNavigationAnalyzerStep["KeyboardNavigationAnalyzerStep"] --> HtmlAggregationStep["HtmlAggregationStep"]
   SemanticCodeAnalyzerStep["SemanticCodeAnalyzerStep"]
   SemanticCodeAnalyzerStep["SemanticCodeAnalyzerStep"] --> HtmlAggregationStep["HtmlAggregationStep"]
   FormValidationAnalyzerStep["FormValidationAnalyzerStep"]
   FormValidationAnalyzerStep["FormValidationAnalyzerStep"] --> HtmlAggregationStep["HtmlAggregationStep"]
   HtmlAggregationStep["HtmlAggregationStep"]
   EndStep["EndStep"]
   EndStep["EndStep"] --> End["End"]
   HtmlAggregationStep["HtmlAggregationStep"] --> EndStep["EndStep"]
   Start --> AltTextAnalyzerStep["AltTextAnalyzerStep"]
   Start --> KeyboardNavigationAnalyzerStep["KeyboardNavigationAnalyzerStep"]
   Start --> SemanticCodeAnalyzerStep["SemanticCodeAnalyzerStep"]
   Start --> FormValidationAnalyzerStep["FormValidationAnalyzerStep"]
*/