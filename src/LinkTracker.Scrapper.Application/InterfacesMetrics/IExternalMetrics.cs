namespace LinkTracker.Scrapper.Application.InterfacesMetrics;

public interface IExternalMetrics
{
    void ObserveScopeDuration(string scope, string scopeType, double ms);
}