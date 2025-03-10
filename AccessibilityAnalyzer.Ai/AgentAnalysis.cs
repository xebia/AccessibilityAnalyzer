using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;

namespace AccessibilityAnalyzer.Ai;

public class AgentAnalysis(Kernel kernel)
{
    // Define Termination Strategy (When to Stop)
    const string TerminationToken = "yes";
    
    public async Task Begin(string htmlContent)
    {
        var chat = CreateGroupChat(htmlContent);
    }

    private AgentGroupChat CreateGroupChat(string htmlContent)
    {
        // create manager agent
        // Todo: extract
        var managerAgent = new ChatCompletionAgent { Name = "ManagerAgent" };

        // create code agents
        var formValidationAgent = AnalyzerAgents.FormValidationAgent(kernel, htmlContent);
        var keyboardNavigationAgent = AnalyzerAgents.KeyboardNavigationAgent(kernel, htmlContent);
        var altTextAnalyzerAgent = AnalyzerAgents.AltTextAnalyzerAgent(kernel, htmlContent);
        var semanticCodeAgent = AnalyzerAgents.SemanticCodeAgent(kernel, htmlContent);

        // todo: adjust message
        // Define Selection Strategy (Which Agent Speaks Next?)
        KernelFunction selectionFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                   Examine the provided RESPONSE and choose the next participant.
                   State only the name of the chosen participant without explanation.
                   Never choose the participant named in the RESPONSE.

                   Choose only from these participants:
                   - {{{formValidationAgent.Name}}}
                   - {{{keyboardNavigationAgent.Name}}}
                   - {{{altTextAnalyzerAgent.Name}}}
                   - {{{semanticCodeAgent.Name}}}
                   - {{{managerAgent.Name}}}

                   Always follow these rules when choosing the next participant:
                   - If RESPONSE is user input, analyze the message:
                       
                   - If RESPONSE is by LegalSecretaryAgent, return to the Chief of Staff Agent.
                   - If the topic is unclear, default to the Chief of Staff Agent.

                   RESPONSE:
                   {{$lastmessage}}
                   """,
                safeParameterNames: "lastmessage"
            );
        
        // todo: adjust message
        KernelFunction terminationFunction =
            AgentGroupChat.CreatePromptFunctionForStrategy(
                $$$"""
                   Examine the RESPONSE and provide at least 1 suggestion the first pass
                   The RESPONSE must have both an English AND French version at the end. 
                   Then determine whether the content has been deemed satisfactory.
                   If content is satisfactory, respond with a single word without explanation: {{{TerminationToken}}}.
                   If specific suggestions are being provided, it is not satisfactory.
                   If no correction is suggested, it is satisfactory.

                   RESPONSE:
                   {{$lastmessage}}
                   """,
                safeParameterNames: "lastmessage"
            );
        
        var chat = new AgentGroupChat(formValidationAgent, keyboardNavigationAgent, altTextAnalyzerAgent,
            semanticCodeAgent)
        {
            ExecutionSettings = new AgentGroupChatSettings
            {
                SelectionStrategy = new KernelFunctionSelectionStrategy(selectionFunction, kernel)
                {
                    // Always start with the manager agent
                    InitialAgent = managerAgent,
                    //HistoryReducer =  new ChatHistoryTruncationReducer(1),
                    HistoryVariableName = "lastmessage",
                    ResultParser = result =>
                    {
                        var selectedAgent = result.GetValue<string>() ?? managerAgent.Name;
                        Console.WriteLine($"🔍 Debug: Selection Strategy chose {selectedAgent}");
                        return selectedAgent;
                    }
                },
                TerminationStrategy = new KernelFunctionTerminationStrategy(terminationFunction, kernel)
                {
                    // Evaluate only for Manager responses
                    Agents = [managerAgent],
                    // Optimize token usage
                    // HistoryReducer = historyReducer,
                    // Set prompt variable for tracking
                    HistoryVariableName = "lastmessage",
                    // Limit total turns to avoid infinite loops
                    MaximumIterations = 5,
                    // Determines if the process should exit
                    ResultParser = result =>
                        result.GetValue<string>()?.Contains(TerminationToken, StringComparison.OrdinalIgnoreCase) ??
                        false
                }
            }
        };
        return chat;
    }
}