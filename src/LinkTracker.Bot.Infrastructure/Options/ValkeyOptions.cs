namespace LinkTracker.Bot.Infrastructure.Options;

public class ValkeyOptions
{
    public const string SectionName = "Valkey";
    public string Configuration { get; set; } = null!;
    public TimeSpan DefaultTtlMinutes { get; set; } = TimeSpan.FromMinutes(10);
    public bool Enabled { get; set; } = true;
}