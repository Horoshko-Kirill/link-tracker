namespace LinkTracker.Bot.Application.InterfacesMetrics;

public interface ICommandMetrics
{
    void IncCommand(string command);
    void ObserveDuration(string command, double ms);
}