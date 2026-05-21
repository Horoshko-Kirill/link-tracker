namespace LinkTracker.AiAgent.Application.Options;

public class FilteringOptions
{
    public List<string> StopWords { get; set; } = null!;
    public List<string> ExcludedAuthors { get; set; } = null!;
    public int MinLength { get; set; } = 10;
}