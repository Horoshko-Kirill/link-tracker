namespace LinkTracker.Bot.Application.Exceptions;

public class ProcessAlreadyExistsException : BotException
{
    public ProcessAlreadyExistsException(string message) : base(message)
    {
    }
}
