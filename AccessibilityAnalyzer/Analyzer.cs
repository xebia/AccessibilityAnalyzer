namespace AccessibilityAnalyzer;

public class Analyzer(SourceGathering.SourceGathering sourceGathering) : IAnalyzer
{
    public async Task<string> AnalyzeUrl(Uri uri)
    {
        var pageData = await sourceGathering.GetPageData(uri);

        if (pageData == null) return string.Empty;

        return pageData.HtmlContent;
    }
}

public interface IAnalyzer
{
    Task<string> AnalyzeUrl(Uri uri);
}