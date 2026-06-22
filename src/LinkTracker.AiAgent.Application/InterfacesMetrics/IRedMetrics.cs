namespace LinkTracker.AiAgent.Application.InterfacesMetrics;

public interface IRedMetrics
{
    void IncRequest(string service, string method, string route, int statusCode);
    void IncError(string service, string method, string route);
    void ObserveDuration(string service, string method, string route, double ms);
}