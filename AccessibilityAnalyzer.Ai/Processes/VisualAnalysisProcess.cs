using AccessibilityAnalyzer.Ai.Steps;
using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai.Processes;

public static class VisualAnalysisProcess
{
    public static ProcessBuilder CreateProcess()
    {
        ProcessBuilder processBuilder = new("VisualAnalysis");

        var uiParserStep = processBuilder.AddStepFromType<UiParserStep>();
        var uiColorAnalysisStep = processBuilder.AddStepFromType<UiColorAnalysisStep>();
        var uiFontsAnalysisStep = processBuilder.AddStepFromType<UiFontsAnalysisStep>();

        var externalStep = processBuilder.AddStepFromType<ExternalVisualAnalysisStep>();

        // Orchestrate the events
        processBuilder
            .OnInputEvent(ProcessEvents.StartVisualAnalysis)
            .SendEventTo(new ProcessFunctionTargetBuilder(uiParserStep));

        uiParserStep
            .OnEvent(UiParserStep.OutputEvents.UiParsed)
            .SendEventTo(new ProcessFunctionTargetBuilder(uiColorAnalysisStep))
            .SendEventTo(new ProcessFunctionTargetBuilder(uiFontsAnalysisStep));

        // todo aggregate step

        uiColorAnalysisStep
            .OnEvent(UiColorAnalysisStep.OutputEvents.UiColorAnalyzed)
            .SendEventTo(new ProcessFunctionTargetBuilder(externalStep));


        return processBuilder;
    }

    public static class ProcessEvents
    {
        public const string StartVisualAnalysis = nameof(StartVisualAnalysis);
        public const string VisualAnalysisReady = nameof(VisualAnalysisReady);
    }

    private sealed class ExternalVisualAnalysisStep() : ExternalStep(ProcessEvents.VisualAnalysisReady);
}