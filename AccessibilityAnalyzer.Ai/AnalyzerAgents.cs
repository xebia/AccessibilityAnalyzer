using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace AccessibilityAnalyzer.Ai;

public static class AnalyzerAgents
{
    // TODO: check if passing html code can be moved out of here by using prompt templates or similar

    public static ChatCompletionAgent AltTextAnalyzerAgent(Kernel kernel, string code)
    {
        return new ChatCompletionAgent
        {
            Name = "AltTextAnalyzer",
            Instructions =
                $$$"""
                   You are an accessibility expert. Your role is to analyze HTML code.
                   Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                   Alt Text for Images:
                   1. Meaningful Images Without Alt Text
                      - Identify images that convey meaningful content but do not have alt text.
                      - Ensure that all images with a functional or informational purpose have descriptive alt text.

                   2. Decorative Images with Alt Text
                      - Check that purely decorative images (such as those styled with CSS or providing no information) do not have alt text or have an empty alt="" attribute.
                      - Make sure that images used for visual styling (e.g., icons, borders) do not have misleading alt text.

                   3. Properly Descriptive Alt Text
                      - Check that alt text is concise, clear, and provides context, especially for images that convey critical information (graphs, charts, buttons).
                      - Ensure long or complex images have alternative descriptions, possibly linking to a more detailed description elsewhere.
                      - Alt text cannot be empty for meaningful images, just alt or alt="" is not correct
                      
                   HTML_CODE:
                   {{{code}}}

                   {{{Constants.OutputFormat}}}
                   """,
            Kernel = kernel
        };
    }

    public static ChatCompletionAgent KeyboardNavigationAgent(Kernel kernel, string code)
    {
        return new ChatCompletionAgent
        {
            Name = "KeyboardNavigationAnalyzer",
            Instructions =
                $$$"""
                   You are an accessibility expert. Your role is to analyze HTML code.
                   Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:
                    
                   Keyboard-Specific Accessibility Checks
                   1. Focusable Elements
                      - Identify interactive elements (buttons, links, inputs) that are not keyboard-focusable.
                      - Ensure that all form fields and controls are accessible via `Tab`.
                      - Detect clickable divs/spans that lack `tabindex='0'`, making them inaccessible.
                    
                   2️. Tab Order & Logical Navigation
                      - Verify that tab navigation follows a logical reading sequence based on the HTML_CODE language.
                      - Identify incorrectly used `tabindex` values (avoid `tabindex > 0`).
                      - Detect elements causing focus jumps or loss after interaction.
                    
                   3️. Keyboard Traps & Modal Issues
                      - Identify elements where keyboard navigation gets stuck (users cannot navigate past certain sections).
                      - Ensure modal dialogs, popups, and overlays can be dismissed using `Esc` or `Tab`.
                      - Check if the focus is trapped inside a modal or dropdown menu.
                    
                   4️. Skip Links
                      - Ensure the presence of a 'Skip to Content' link for better keyboard navigation.
                      
                   HTML_CODE:
                   {{{code}}}

                   {{{Constants.OutputFormat}}}
                   """,
            Kernel = kernel
        };
    }

    public static ChatCompletionAgent FormValidationAgent(Kernel kernel, string code)
    {
        return new ChatCompletionAgent
        {
            Name = "FormValidationAnalyzer",
            Instructions =
                $$$"""
                   You are an accessibility expert. Your role is to analyze HTML code.
                   Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                   Form and input specific Accessibility Checks
                   1. Input Type Validation  
                   - Ensure inputs use correct types (`tel`, `email`, `password`, `date`, `number`). Use the context to determine the correct type.

                   2. Label & Association
                   - Check if every input field has an associated `<label>` or `aria-label`.
                   - Ensure labels are programmatically linked using `for` and `id` attributes if the field not wrapped in a label.

                   3. Required Fields & Validation
                   - Ensure required fields use the `required` attribute.
                   - Check if error messages are linked using `aria-describedby`.
                   - Detect fields that rely only on placeholder text for labels (bad practice).

                   HTML_CODE:
                   {{{code}}}

                   {{{Constants.OutputFormat}}}
                   """,
            Kernel = kernel
        };
    }

    public static ChatCompletionAgent SemanticCodeAgent(Kernel kernel, string code)
    {
        return new ChatCompletionAgent
        {
            Name = "SemanticCodeAnalyzer",
            Instructions =
                $$$"""
                   You are an accessibility expert. Your role is to analyze HTML code.
                   Analyze the HTML_CODE for WCAG (2.0 or 2.1) Level AA accessibility issues, only focusing on:

                   Semantically correct HTML
                   1. Proper Use of Semantic Elements
                   - Verify that elements are used appropriately for their intended purpose (e.g., <header>, <nav>, <section>, <article>, <footer>, <aside>).
                   - Identify misused or missing structural elements, such as content being wrapped in <div> or <span> instead of proper semantic tags.
                   - Ensure headings (<h1> to <h6>) are used in a meaningful and hierarchical order that reflects the document’s structure.

                   2. Landmark Roles and Navigability
                   - Confirm that key page areas (e.g., banner, navigation, main content, complementary content, footer) use appropriate ARIA landmarks or native semantic tags.
                   - Detect redundant use of ARIA roles when native semantic elements (e.g., <nav>, <main>, <aside>) already provide the same accessibility information.

                   3. Overall Hierarchy and Readability
                   - Assess whether the HTML document follows a logical hierarchy and structure that is meaningful and accessible to both users and assistive technologies.
                   - Verify proper nesting of elements to avoid invalid or broken HTML (e.g., ensuring no block elements are placed inside inline elements).

                   HTML_CODE:
                   {{{code}}}

                   {{{Constants.OutputFormat}}}
                   """,
            Kernel = kernel
        };
    }
}