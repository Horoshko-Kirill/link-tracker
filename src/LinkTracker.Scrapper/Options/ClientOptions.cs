namespace LinkTracker.Scrapper.Options;

public class ClientOptions
{
    public const string SectionName = "ClientType";
    public string Type { get; set; } = "Http";
}