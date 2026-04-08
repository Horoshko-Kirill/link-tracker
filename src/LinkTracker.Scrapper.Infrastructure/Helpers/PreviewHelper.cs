namespace LinkTracker.Scrapper.Infrastructure.Helpers;

public static class PreviewHelper
{
    public static string Build(string? text, int maxLength = 200)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalized = text.Trim();

        return normalized.Length <= maxLength
            ? normalized
            : normalized[..maxLength];
    }
}