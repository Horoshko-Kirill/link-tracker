namespace LinkTracker.Bot.Infrastructure.Options;

/// <summary>
/// Класс для типобезопасной конфигурации
/// </summary>
public class BotOptions
{
    public const string SectionName = "Bot";
    public string Token { get; set; } = string.Empty;
}
