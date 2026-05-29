namespace LinkTracker.Scrapper.Infrastructure.Helpers;

public static class GetDomainHelper
{
    public static string GetDomain(string url)
    {
        try
        {
            return new Uri(url).Host;
        }
        catch
        {
            return "unknown";
        }
    }
}