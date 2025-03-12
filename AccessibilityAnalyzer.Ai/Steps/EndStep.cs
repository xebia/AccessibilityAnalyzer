using System.Text.Json;
using AccessibilityAnalyzer.Ai.Models;
using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai.Steps;

public class EndStep : KernelProcessStep<EndStep.FinalState>
{
    private FinalState _state;

    public override ValueTask ActivateAsync(KernelProcessStepState<FinalState> state)
    {
        _state = state.State!;
        return base.ActivateAsync(state);
    }

    [KernelFunction(Functions.End)]
    public async Task<AccessibilityAnalysis[]?> End(Kernel kernel, KernelProcessStepContext context, string response)
    {
        var accessibilityAnalyses = JsonSerializer.Deserialize<AccessibilityAnalysis[]>(response,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (accessibilityAnalyses == null || accessibilityAnalyses.Length == 0)
        {
            await context.EmitEventAsync("Error", "No accessibility issues were found.");
            return null;
        }

        _state.Result = accessibilityAnalyses;
        return accessibilityAnalyses;
    }

    public class Functions
    {
        public const string End = nameof(End);
    }

    public class FinalState
    {
        public AccessibilityAnalysis[] Result { get; set; }
    }
}