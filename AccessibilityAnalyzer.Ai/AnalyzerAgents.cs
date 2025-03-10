using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace AccessibilityAnalyzer.Ai;

public static class AnalyzerAgents
{
    public static ChatCompletionAgent AltTextAnalyzerAgent(Kernel kernel) => new()
    {
        Name = "AltTextAnalyzer",
        Instructions =
            """
            
            """,
        Kernel = kernel,
    };
    
    public static ChatCompletionAgent KeyboardNavigationAgent(Kernel kernel) => new()
    {
        Name = "KeyboardNavigationAnalyzer",
        Instructions =
            """
            
            """,
        Kernel = kernel,
    };
    
    public static ChatCompletionAgent FormValidationAgent(Kernel kernel) => new()
    {
        Name = "FormValidationAnalyzer",
        Instructions =
            """
            
            """,
        Kernel = kernel,
    };
    
    public static ChatCompletionAgent SemanticCodeAgent(Kernel kernel) => new()
    {
        Name = "SemanticCodeAnalyzer",
        Instructions =
            """
            
            """,
        Kernel = kernel,
    };
}