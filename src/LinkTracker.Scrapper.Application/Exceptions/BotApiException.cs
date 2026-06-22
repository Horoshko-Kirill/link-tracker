namespace LinkTracker.Scrapper.Application.Exceptions;

public class BotApiException : Exception
{
    public BotApiException(string message) : base(message)
    {
    }
}
