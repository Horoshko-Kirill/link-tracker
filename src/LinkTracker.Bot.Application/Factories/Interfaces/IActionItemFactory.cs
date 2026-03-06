using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Factories.Interfaces;

public interface IActionItemFactory
{
    public string ProcessType { get; }
    ActionItem CreateInitialAction(Process process);

}
