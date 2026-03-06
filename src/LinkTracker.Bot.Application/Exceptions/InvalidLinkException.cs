namespace LinkTracker.Bot.Application.Exceptions;

public class InvalidLinkException : BotException
{
    public InvalidLinkException()
    {
    }

    public InvalidLinkException(string message) : base(message)
    {
    }
}
