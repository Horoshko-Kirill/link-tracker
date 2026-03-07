namespace LinkTracker.Bot.Constans;

/// <summary>
/// Статический класс для хранения константой строки для команды /help
/// </summary>
public static class HelpConstants
{
    public static string constants = "" +
        "/start - Начать работу\n" +
        "/track - Начать отслеживание ссылки\n" +
        "/untrack - Прекратить отслеживание ссылки\n" +
        "/list [тег] - Вывести список отслеживаемых ссылок с фильтрацией по тегу\n" +
        "/help - Показать список команд\n";
}
