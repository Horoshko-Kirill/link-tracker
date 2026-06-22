namespace LinkTracker.Bot.Application.Exceptions;

public abstract class BotException : Exception
{
    protected BotException(string message) : base(message)
    {
    }
}
