using AccessibilityAnalyzer.Ai;
using AccessibilityAnalyzer.Ai.Models;

namespace AccessibilityAnalyzer;

public class Analyzer(SourceGathering.SourceGathering sourceGathering, IAnalysisProcess analysisProcess) : IAnalyzer
{
    public async Task<AccessibilityAnalysis[]?> AnalyzeUrl(Uri uri)
    {
        var pageData = await sourceGathering.GetPageData(uri);

        if (pageData == null) return null;

        return await analysisProcess.StartProcess(pageData.HtmlContent, pageData.DesktopScreenshot);
    }
}

public interface IAnalyzer
{
    Task<AccessibilityAnalysis[]?> AnalyzeUrl(Uri uri);
}