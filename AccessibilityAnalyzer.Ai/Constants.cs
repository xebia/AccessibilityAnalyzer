namespace AccessibilityAnalyzer.Ai;

public static class Constants
{
    public const string OutputFormat = """
                                       OUTPUT FORMAT:
                                       Return only structured JSON in the following example format. Not in markdown. Just pure JSON as text. Add more objects as needed:
                                        
                                       {
                                           ""analysis"": [
                                               {
                                                   ""id"": ""2.1.1"", (WCAG success criterion reference, e.g., “1.1.1”)
                                                   ""error"": ""Code.Keyboard.Focusable"", (unique error code, Code if html was analyzed, Visual if screenshot was analyzed)
                                                   ""category"": ""Keyboard Accessibility"", (category of the agent performing the analysis)
                                                   ""description"": ""Element is not keyboard-focusable."", (Short description of the issue)
                                                   ""detail"": ""All interactive elements must be accessible via keyboard navigation."", (Detailed description of the issue)
                                                   ""information"": [
                                                       {
                                                           ""title"": ""How to fix"",
                                                           ""description"": ""Ensure that all interactive elements are focusable via the keyboard."" (Suggested solution to the issue)
                                                       },
                                                       {
                                                           ""title"": ""Part of WCAG 2.1"", (WCAG success criterion reference, e.g., “1.1.1”)
                                                           ""description"": ""2.1.1 Keyboard"" (WCAG success criterion title)
                                                       }
                                                   ]
                                               },
                                               (add more objects as needed)
                                           ],
                                           ""summary"": {
                                               ""failed"": {failed_count} (total number of failed checks),
                                           }
                                       }
                                       """;
}