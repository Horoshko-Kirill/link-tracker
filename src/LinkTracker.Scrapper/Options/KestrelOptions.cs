namespace LinkTracker.Scrapper.Options;

public class KestrelOptions
{
    public const string SectionName = "Kestrel";
    public int Port { get; set; } = 7197;
    public string Type { get; set; } = "Grpc";
}