using LinkTracker.AiAgent.Application.InterfacesFactory;
using LinkTracker.AiAgent.Application.InterfacesServices;
using Microsoft.Extensions.Logging;

namespace LinkTracker.AiAgent.Infrastructure.Services;

public class ResilientSummarizer : ISummarizer
{
    private readonly ISummarizer _inner;
    private readonly StubSummarizer _stub;
    private readonly ILogger<ResilientSummarizer> _logger;

    public ResilientSummarizer(ISummarizerFactory factory, StubSummarizer stub, ILogger<ResilientSummarizer> logger)
    {
        _inner = factory.Create();
        _stub = stub;
        _logger = logger;
    }

    public async Task<string> SummarizeAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _inner.SummarizeAsync(text, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI summarization failed");

            return await _stub.SummarizeAsync(text, cancellationToken);
        }
    }
}