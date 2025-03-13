using AccessibilityAnalyzer.Ai.Models;
using AccessibilityAnalyzer.Ai.Steps;
using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai;

public interface IAnalysisProcess
{
    Task<AccessibilityAnalysis[]?> Begin(string htmlContent);
}

public class AnalysisProcess(Kernel kernel) : IAnalysisProcess
{
    public async Task<AccessibilityAnalysis[]?> Begin(string htmlContent)
    {
        ProcessBuilder processBuilder = new("HtmlContentAnalysis");

        var altTextAnalyzerStep = processBuilder.AddStepFromType<AltTextAnalyzerStep>();
        var keyboardNavigationAnalyzerStep = processBuilder.AddStepFromType<KeyboardNavigationAnalyzerStep>();
        var semanticCodeAnalyzerStep = processBuilder.AddStepFromType<SemanticCodeAnalyzerStep>();
        var formValidationAnalyzerStep = processBuilder.AddStepFromType<FormValidationAnalyzerStep>();
        var htmlAggregationStep = processBuilder.AddStepFromType<HtmlAggregationStep>();
        var endStep = processBuilder.AddStepFromType<EndStep>();

        var uiAnalyzerStep = processBuilder.AddStepFromType<UiAnalyzerStep>();

        // Orchestrate the events
        processBuilder
            .OnInputEvent("Start")
            //.SendEventTo(new ProcessFunctionTargetBuilder(uiAnalyzerStep));
            .SendEventTo(new ProcessFunctionTargetBuilder(altTextAnalyzerStep))
            .SendEventTo(new ProcessFunctionTargetBuilder(keyboardNavigationAnalyzerStep))
            .SendEventTo(new ProcessFunctionTargetBuilder(semanticCodeAnalyzerStep))
            .SendEventTo(new ProcessFunctionTargetBuilder(formValidationAnalyzerStep));

        altTextAnalyzerStep
            .OnEvent(AltTextAnalyzerStep.OutputEvents.AltTextAnalyzed)
            .SendEventTo(new ProcessFunctionTargetBuilder(htmlAggregationStep,
                HtmlAggregationStep.Functions.AltTextWasAnalyzed));

        keyboardNavigationAnalyzerStep
            .OnEvent(KeyboardNavigationAnalyzerStep.OutputEvents.KeyboardNavigationAnalyzed)
            .SendEventTo(new ProcessFunctionTargetBuilder(htmlAggregationStep,
                HtmlAggregationStep.Functions.KeyboardNavigationWasAnalyzed));

        semanticCodeAnalyzerStep
            .OnEvent(SemanticCodeAnalyzerStep.OutputEvents.SemanticStructureAnalyzed)
            .SendEventTo(new ProcessFunctionTargetBuilder(htmlAggregationStep,
                HtmlAggregationStep.Functions.SemanticStructureWasAnalyzed));

        formValidationAnalyzerStep
            .OnEvent(FormValidationAnalyzerStep.OutputEvents.FormValidationAnalyzed)
            .SendEventTo(new ProcessFunctionTargetBuilder(htmlAggregationStep,
                HtmlAggregationStep.Functions.FormValidationWasAnalyzed));

        htmlAggregationStep
            .OnEvent(HtmlAggregationStep.OutputEvents.AnalysisComplete)
            .SendEventTo(new ProcessFunctionTargetBuilder(endStep));

        endStep
            .OnFunctionResult()
            .StopProcess();


        var process = processBuilder.Build();

        var processContext =
            await process.StartAsync(kernel, new KernelProcessEvent { Id = "Start", Data = htmlContent });


        var processInfo = await processContext.GetStateAsync();
        var step = processInfo.Steps.FirstOrDefault(s => s.State.Name == nameof(EndStep));
        if (step == null) return null;

        var state = step.State as KernelProcessStepState<EndStep.FinalState>;
        return state!.State!.Result;
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