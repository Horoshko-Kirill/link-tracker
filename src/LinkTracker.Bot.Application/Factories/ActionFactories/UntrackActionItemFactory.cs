using LinkTracker.Bot.Application.Factories.Interfaces;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Factories.ActionFactories;

public class UntrackActionItemFactory : IActionItemFactory
{
    public string ProcessType => "Untrack";

    public ActionItem CreateInitialAction(Process process)
    {
        return new ActionItem
        {
            ProcessId = process.Id,
            ActionType = ActionType.AwaitingUntrackLink
        };
    }
}
