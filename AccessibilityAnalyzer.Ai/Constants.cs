namespace AccessibilityAnalyzer.Ai;

public struct Constants
{
    public const string ImageMimeType = "image/jpeg";
    public const string OutputFormat = """
                                       OUTPUT FORMAT:
                                       Return only structured JSON in the following example format. 
                                       You can only answer with valid json. Only add a maximum of 3 objects. Use the most important ones.
                                       Don't use indentation or new lines in the JSON string.
                                        
                                       {
                                         "analysis": [
                                           {
                                             "id": "2.1.1", (WCAG success criterion reference, e.g., "1.1.1")
                                             "error": "Code.Keyboard.Focusable", (unique error code, Code if html was analyzed, Visual if screenshot was analyzed)
                                             "category": "Keyboard Accessibility", (category of the agent performing the analysis)
                                             "description": "Element is not keyboard-focusable.", (Very short description of the issue, acts as the title)
                                             "detail": "All interactive elements must be accessible via keyboard navigation.", (Description of the issue)
                                           },
                                           (add more objects as needed)
                                         ],
                                         "summary": {
                                           "failed": {failed_count} (total number of failed checks),
                                         }
                                       }
                                       """;
    
    // public const string OutputFormatOld = """
    //                                    OUTPUT FORMAT:
    //                                    Return only structured JSON in the following example format. 
    //                                    You can only answer with valid json. Add more objects as needed:
                                        
    //                                    {
    //                                        ""analysis"": [
    //                                            {
    //                                                ""id"": ""2.1.1"", (WCAG success criterion reference, e.g., “1.1.1”)
    //                                                ""error"": ""Code.Keyboard.Focusable"", (unique error code, Code if html was analyzed, Visual if screenshot was analyzed)
    //                                                ""category"": ""Keyboard Accessibility"", (category of the agent performing the analysis)
    //                                                ""description"": ""Element is not keyboard-focusable."", (Short description of the issue)
    //                                                ""detail"": ""All interactive elements must be accessible via keyboard navigation."", (Detailed description of the issue)
    //                                                ""information"": [
    //                                                    {
    //                                                        ""title"": ""How to fix"",
    //                                                        ""description"": ""Ensure that all interactive elements are focusable via the keyboard."" (Suggested solution to the issue)
    //                                                    },
    //                                                    {
    //                                                        ""title"": ""Part of WCAG 2.1"", (WCAG success criterion reference, e.g., “1.1.1”)
    //                                                        ""description"": ""2.1.1 Keyboard"" (WCAG success criterion title)
    //                                                    }
    //                                                ]
    //                                            },
    //                                            (add more objects as needed)
    //                                        ],
    //                                        ""summary"": {
    //                                            ""failed"": {failed_count} (total number of failed checks),
    //                                        }
    //                                    }
    //                                    """;
}