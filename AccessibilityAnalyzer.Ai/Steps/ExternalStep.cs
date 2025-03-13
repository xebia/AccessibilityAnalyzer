using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Ai.Steps;

public class ExternalStep(string externalEventName) : KernelProcessStep
{
    [KernelFunction]
    public async Task EmitExternalEventAsync(KernelProcessStepContext context, object data)
    {
        await context.EmitEventAsync(new KernelProcessEvent
            { Id = externalEventName, Data = data, Visibility = KernelProcessEventVisibility.Public });
    }
}