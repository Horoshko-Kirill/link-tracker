using LinkTracker.AiAgent.Application.InterfacesServices;

namespace LinkTracker.AiAgent.Application.InterfacesFactory;

public interface ISummarizerFactory
{
    ISummarizer Create();
}