namespace AccessibilityAnalyzer;

public class Analyzer : IAnalyzer
{
    public string AnalyzeUrl(string url)
    {
        return url;
    }
}

public interface IAnalyzer
{
    string AnalyzeUrl(string url);
}