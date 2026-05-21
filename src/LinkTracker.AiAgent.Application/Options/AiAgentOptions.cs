namespace LinkTracker.AiAgent.Application.Options;

public class AiAgentOptions
{
    public const string SectionName = "AiAgent";
    public FilteringOptions FilteringOptions { get; set; } = null!;
    public SummarizationOptions SummarizationOptions { get; set; } = null!;
}