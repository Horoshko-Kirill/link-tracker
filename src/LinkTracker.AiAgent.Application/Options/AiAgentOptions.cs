namespace LinkTracker.AiAgent.Application.Options;

public class AiAgentOptions
{
    public const string SectionName = "AiAgent";
    public FilteringOptions Filtering { get; set; } = null!;
    public SummarizationOptions Summarization { get; set; } = null!;
    public GroupingOptions Grouping { get; set; } = null!;
    public PrioritizationOptions Prioritization { get; set; } = null!;
}