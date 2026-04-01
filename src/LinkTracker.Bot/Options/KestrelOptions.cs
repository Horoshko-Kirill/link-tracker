namespace LinkTracker.Bot.Options;

public class KestrelOptions
{
    public const string SectionName = "Kestrel";
    public int Port { get; set; } = 7175;
    public string Type { get; set; } = "Grpc";
}