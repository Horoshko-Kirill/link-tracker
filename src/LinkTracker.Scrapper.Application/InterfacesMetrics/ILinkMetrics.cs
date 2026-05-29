namespace LinkTracker.Scrapper.Application.InterfacesMetrics;

public interface ILinkMetrics
{
    void IncLinks(string domain);
    void DecLinks(string domain);
}