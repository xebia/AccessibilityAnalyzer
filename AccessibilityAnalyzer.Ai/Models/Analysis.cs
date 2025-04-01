namespace AccessibilityAnalyzer.Ai.Models;

public class AccessibilityAnalysis
{
    public List<AnalysisResult> Analysis { get; set; }
    public Summary Summary { get; set; }
}

public class AnalysisResult
{
    public string Id { get; set; }
    public string Error { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Detail { get; set; }
    public string Location { get; set; }
    public List<Information> Information { get; set; }
}

public class Information
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class Summary
{
    public int Failed { get; set; }
}