using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;

namespace AccessibilityAnalyzer.Ai;

public class OmniParserLocalService : IImageToTextService
{
    private readonly OmniParserClient _client;

    public OmniParserLocalService(Uri endpoint, HttpClient httpClient)
    {
        _client = new OmniParserClient(endpoint, httpClient);
    }

    public IReadOnlyDictionary<string, object?> Attributes { get; }

    public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(ImageContent content,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null, CancellationToken cancellationToken = new())
    {
        return await _client.GenerateTextFromImageAsync(content, cancellationToken);
    }
}

public class OmniParserClient(Uri endpoint, HttpClient httpClient)
{
    public async Task<IReadOnlyList<TextContent>> GenerateTextFromImageAsync(ImageContent content,
        CancellationToken cancellationToken)
    {
        using var httpRequestMessage = CreateImageToTextRequest(content);
        var body = await SendRequestAndGetStringBodyAsync(httpRequestMessage, cancellationToken);

        // var response = System.Text.Json.JsonSerializer.Deserialize<OmniParserResult>(body);
        var response = new TextContent(body);
        return [response];
    }

    private async Task<string> SendRequestAndGetStringBodyAsync(HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private HttpRequestMessage CreateImageToTextRequest(ImageContent content)
    {
        // Read the file into a byte array
        var imageContent = new ByteArrayContent(content.Data?.ToArray() ?? []);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue(content.MimeType ?? string.Empty);

        var payload = new ParseRequest(content.Data!.Value.ToArray());
        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(payload);

        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new ByteArrayContent(jsonBytes)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return request;
    }

    private class ParseRequest
    {
        public ParseRequest(byte[] payload)
        {
            Base64Image = payload;
        }

        [JsonPropertyName("base64_image")] public byte[] Base64Image { get; set; }
    }
}