namespace LinkTracker.Bot.Application.Exceptions;

public class ProcessNotFoundException : BotException
{
    public ProcessNotFoundException(string message) : base(message)
    {
    }
}
