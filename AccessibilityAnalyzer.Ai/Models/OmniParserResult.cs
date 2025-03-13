using System.Text.Json.Serialization;

namespace AccessibilityAnalyzer.Ai.Models;

public class OmniParserResult
{
    [JsonPropertyName("som_image_base64")] public string SomImageBase64 { get; set; }

    [JsonPropertyName("parsed_content_list")]
    public ParsedContent[] ParsedContentList { get; set; }

    public double Latency { get; set; }
}

public class ParsedContent
{
    public string Type { get; set; }

    [JsonPropertyName("bbox")] public double[] BoundingBox { get; set; } // [x, y, width, height]

    public bool Interactivity { get; set; }

    public string Content { get; set; }

    public string Source { get; set; }
}