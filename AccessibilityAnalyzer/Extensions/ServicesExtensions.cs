using AccessibilityAnalyzer.Ai;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;

namespace AccessibilityAnalyzer.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddOpenAiServices(this IServiceCollection serviceCollection)
    {
        var gpt3DeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_GPT3_NAME");
        var gpt4DeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_GPT4_NAME");
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var key = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");

        if (gpt3DeploymentName is null || gpt4DeploymentName is null || endpoint is null || key is null)
            throw new InvalidOperationException("Missing OpenAI configuration");
        
        serviceCollection.AddAzureOpenAIChatCompletion(
            gpt3DeploymentName,
            apiKey: key,
            endpoint: endpoint,
            serviceId: Constants.Gpt3ServiceKey
        );
        
        serviceCollection.AddAzureOpenAIChatCompletion(
            gpt4DeploymentName,
            apiKey: key,
            endpoint: endpoint,
            serviceId: Constants.Gpt4ServiceKey
        );

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