namespace LinkTracker.Bot.Infrastructure.Kafka.Result;

public record ProcessingResult(
    bool IsSuccess,
    string? ErrorType = null,
    string? ErrorMessage = null)
{
    public static ProcessingResult Success()
    {
        return new ProcessingResult(true);
    }

    public static ProcessingResult Failed(string errorType, string errorMessage)
    {
        return new ProcessingResult(false, errorType, errorMessage);
    }
}