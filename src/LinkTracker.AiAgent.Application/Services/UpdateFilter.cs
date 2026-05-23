using LinkTracker.AiAgent.Application.InterfacesServices;
using LinkTracker.AiAgent.Application.Options;
using LinkTracker.AiAgent.Domain.Models;
using Microsoft.Extensions.Options;

namespace LinkTracker.AiAgent.Application.Services;

public class UpdateFilter : IUpdateFilter
{
    private readonly AiAgentOptions _options;

    public UpdateFilter(IOptions<AiAgentOptions> options)
    {
        _options = options.Value;
    }
    
    public bool ShouldProcess(RawLinkUpdate update)
    {
        if (update.Description.Length < _options.Filtering.MinLength)
        {
            return false;
        }

        var text = update.Description.ToLowerInvariant();

        if (_options.Filtering.ExcludedAuthors
            .Any(a => text.Contains(a.ToLowerInvariant())))
        {
            return false;
        }

        if (_options.Filtering.StopWords
            .Any(w => text.Contains(w.ToLowerInvariant())))
        {
            return false;
        }

        return true;
    }
}