namespace LinkTracker.Bot.Contracts.Dto
{
    public class ApiErrorResponse
    {
        public string Code { get; set; } = null!;
        public string? ExceptionName { get; set; }
        public string? ExceptionMessage { get; set; }
        public List<string>? StackTrace { get; set; }
    }
}
