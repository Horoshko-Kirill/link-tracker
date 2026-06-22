namespace LinkTracker.Bot.Application.Exceptions;

public class MessageNullException : BotException
{
    public MessageNullException(string message) : base(message)
    {
    }
}
