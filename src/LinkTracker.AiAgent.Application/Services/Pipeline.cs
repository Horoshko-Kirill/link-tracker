using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Domain.Enums;
using LinkTracker.AiAgent.Domain.Models;

namespace LinkTracker.AiAgent.Application.Services;

public class Pipeline : IPipeline
{
    private readonly IUpdateFilter _filter;
    private readonly IPriority _priority;
    private readonly ISummarizer _summarizer;

    public Pipeline(IUpdateFilter filter, ISummarizer summarizer, IPriority priority)
    {
        _filter = filter;
        _summarizer = summarizer;
        _priority = priority;
    }

    public async Task<ProcessedLinkUpdate?> ProcessAsync(RawLinkUpdate update, CancellationToken cancellationToken = default)
    {
        if (!_filter.ShouldProcess(update))
        {
            return null;
        }

        var priority = _priority.GetPriorityLevel(update.Description);
        
        var text = await _summarizer.SummarizeAsync(update.Description, cancellationToken);

        var result = new ProcessedLinkUpdate
        {
            EventId = update.EventId,
            Url = update.Url,
            Description = text,
            ChatIds = update.ChatIds,
            PriorityLevel = priority
        };

        return result;
    }
}