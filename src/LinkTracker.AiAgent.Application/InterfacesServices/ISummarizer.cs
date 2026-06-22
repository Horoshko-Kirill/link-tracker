namespace LinkTracker.AiAgent.Application.InterfacesServices;

public interface ISummarizer
{
    Task<string> SummarizeAsync(string text, CancellationToken cancellationToken = default);
}