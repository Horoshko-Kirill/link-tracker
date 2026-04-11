namespace LinkTracker.Scrapper.Application.Common.Results;

public class LinkProcessingResult
{
    public long LinkId { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public static LinkProcessingResult Ok()
    {
        return new LinkProcessingResult { Success = true };
    }

    public static LinkProcessingResult Error(long linkId, string url, string errorMessage)
    {
        return new LinkProcessingResult { LinkId = linkId, Url = url, Success = false, ErrorMessage = errorMessage };
    }
}