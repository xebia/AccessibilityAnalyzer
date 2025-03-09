using Microsoft.SemanticKernel;

namespace AccessibilityAnalyzer.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOpenAiServices(this IServiceCollection serviceCollection)
    {
        var gptDeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_GPT_NAME");
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

        if (gptDeploymentName is null || endpoint is null || key is null)
            throw new InvalidOperationException("Missing OpenAI configuration");

        serviceCollection.AddAzureOpenAIChatCompletion(
            gptDeploymentName,
            apiKey: key,
            endpoint: endpoint,
            modelId: "gpt-4o" // Optional name of the underlying model if the deployment name doesn't match the model name
        );
#pragma warning disable SKEXP0001
        //serviceCollection.AddAzureOpenAITextToAudio(ttsDeploymentName, endpoint, key);
#pragma warning restore SKEXP0001

        serviceCollection.AddTransient(serviceProvider => new Kernel(serviceProvider));

        return serviceCollection;
    }
}