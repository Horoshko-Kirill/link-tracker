namespace LinkTracker.Bot.Application.Exceptions;

public class ProcessLimitExceededException : BotException
{
    public ProcessLimitExceededException(string message) : base(message)
    {
    }
}
