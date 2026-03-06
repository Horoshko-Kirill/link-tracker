namespace LinkTracker.Bot.Application.Exceptions;

public class InvalidStateException : Exception
{
    public InvalidStateException(string message) : base(message)
    {
    }
}
