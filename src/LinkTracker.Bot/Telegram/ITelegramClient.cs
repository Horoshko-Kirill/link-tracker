using LinkTracker.Bot.Commands.Interfaces;
using Telegram.Bot.Types;

namespace LinkTracker.Bot.Telegram;
/// <summary>
/// Интерфейс реализующий обертку над библиотекой, предоставляющей методы телеграмма
/// </summary>
public interface ITelegramClient
{
    /// <summary>
    /// Отправка сообщения
    /// </summary>
    /// <param name="chatId"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendMessageAsync(long chatId, string message, CancellationToken cancellationToken);

    /// <summary>
    /// Устанавливаем доступные команды
    /// </summary>
    /// <param name="commands"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SetCommandsAsync(IEnumerable<ICommand> commands, CancellationToken cancellationToken);

    /// <summary>
    /// Запускаем отслеживание(получение) обновлений телеграмма
    /// </summary>
    /// <param name="handleUpdate"></param>
    /// <param name="cancellationToken"></param>
    void StartReceivingAsync(Func<Update, Task> handleUpdate, CancellationToken cancellationToken);
}
