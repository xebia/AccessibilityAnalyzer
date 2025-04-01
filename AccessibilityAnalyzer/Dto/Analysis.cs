namespace AccessibilityAnalyzer.Dto;

public class AccessibilityAnalysis
{
    public List<AnalysisResult> Analysis { get; set; }
    public Summary Summary { get; set; }

    public static AccessibilityAnalysis FromModel(Ai.Models.AccessibilityAnalysis model)
    {
        return new AccessibilityAnalysis
        {
            Analysis = model.Analysis?.Select(item => new AnalysisResult
            {
                Id = item.Id,
                Error = item.Error,
                Category = item.Category,
                Description = item.Description,
                Detail = item.Detail,
                Location = item.Location,
                Information = item.Information?.Select(info => new Information
                {
                    Title = info.Title,
                    Description = info.Description
                }).ToList() ?? new List<Information>()
            }).ToList() ?? new List<AnalysisResult>(),

            Summary = new Summary
            {
                Failed = model.Summary?.Failed ?? 0
            }
        };
    }
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