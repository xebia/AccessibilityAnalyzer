namespace AccessibilityAnalyzer;

public class Analyzer(SourceGathering.SourceGathering sourceGathering) : IAnalyzer
{
    public async Task<string> AnalyzeUrl(string url)
    {
        var pageData = await sourceGathering.GetPageData(url);

        if (pageData == null) return string.Empty;

        return pageData.HtmlContent;
    }
}

public interface IAnalyzer
{
    Task<string> AnalyzeUrl(string url);
}