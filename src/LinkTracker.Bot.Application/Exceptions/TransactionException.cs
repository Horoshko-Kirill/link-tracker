namespace LinkTracker.Bot.Application.Exceptions;

public class TransactionException : Exception
{
    public TransactionException(string message) : base(message)
    {
    }
}