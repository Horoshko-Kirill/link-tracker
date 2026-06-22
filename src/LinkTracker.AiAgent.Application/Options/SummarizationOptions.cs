namespace LinkTracker.AiAgent.Application.Options;

public class SummarizationOptions
{
    public int Threshold { get; set; } = 100;
    public string Provider { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
