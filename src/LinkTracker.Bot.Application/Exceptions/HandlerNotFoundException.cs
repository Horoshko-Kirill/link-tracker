namespace LinkTracker.Bot.Application.Exceptions;

public class HandlerNotFoundException : BotException
{
    public HandlerNotFoundException(string message) : base(message)
    {
    }
}
