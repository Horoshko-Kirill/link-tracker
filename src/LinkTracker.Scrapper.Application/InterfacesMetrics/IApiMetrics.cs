namespace LinkTracker.Scrapper.Application.InterfacesMetrics;

public interface IApiMetrics
{
    void IncRequest(string source, string method, string route);
    void ObserveDuration(string source, string method, string route, double ms);
}