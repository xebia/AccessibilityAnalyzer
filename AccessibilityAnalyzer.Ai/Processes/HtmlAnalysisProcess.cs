using AccessibilityAnalyzer.Ai.Steps;
using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai.Processes;

public static class HtmlAnalysisProcess
{
    public static ProcessBuilder CreateProcess()
    {
        ProcessBuilder processBuilder = new("HtmlContentAnalysis");

        var altTextAnalyzerStep = processBuilder.AddStepFromType<AltTextAnalyzerStep>();
        var keyboardNavigationAnalyzerStep = processBuilder.AddStepFromType<KeyboardNavigationAnalyzerStep>();
        var semanticCodeAnalyzerStep = processBuilder.AddStepFromType<SemanticCodeAnalyzerStep>();
        var formValidationAnalyzerStep = processBuilder.AddStepFromType<FormValidationAnalyzerStep>();
        var htmlAggregationStep = processBuilder.AddStepFromType<HtmlAggregationStep>();
        var endStep = processBuilder.AddStepFromType<EndStep>();
        var externalStep = processBuilder.AddStepFromType<ExternalHtmlAnalysisStep>();

        // Orchestrate the events
        processBuilder
            .OnInputEvent(ProcessEvents.StartHtmlContentAnalysis)
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
            .OnEvent(EndStep.OutputEvents.ResponseParsed)
            .SendEventTo(new ProcessFunctionTargetBuilder(externalStep));
        
        return processBuilder;
    }

    public static class ProcessEvents
    {
        public const string StartHtmlContentAnalysis = nameof(StartHtmlContentAnalysis);
        public const string HtmlContentAnalysisReady = nameof(HtmlContentAnalysisReady);
    }

    private sealed class ExternalHtmlAnalysisStep() : ExternalStep(ProcessEvents.HtmlContentAnalysisReady);
}