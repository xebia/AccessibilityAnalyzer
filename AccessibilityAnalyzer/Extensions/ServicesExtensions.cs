using AccessibilityAnalyzer.Ai;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;

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
            endpoint: endpoint
        );

        var huggingFaceApiToken = Environment.GetEnvironmentVariable("HUGGINGFACE_API_KEY");
        if (huggingFaceApiToken is null)
            throw new InvalidOperationException("Missing HuggingFace configuration");

        // serviceCollection.AddHuggingFaceImageToText(
        //     model: "microsoft/OmniParser-v2.0",
        //     apiKey: huggingFaceApiToken
        // );  

        serviceCollection.AddLocalOmniParserImageToText(
            new Uri("http://localhost:8000/parse/")
        );

        serviceCollection.AddTransient(serviceProvider => new Kernel(serviceProvider));

        return serviceCollection;
    }

    public static IServiceCollection AddAiAnalysis(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IAnalysisProcess, AnalysisProcess>();

        return serviceCollection;
    }

    public static IServiceCollection AddLocalOmniParserImageToText(
        this IServiceCollection services,
        Uri endpoint)
    {
        return services.AddSingleton<IImageToTextService>(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            return new OmniParserLocalService(
                endpoint,
                httpClientFactory.CreateClient(nameof(OmniParserLocalService))
            );
        });
    }
}