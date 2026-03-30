using LinkTracker.Bot.Application.Factories.Interfaces;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;

namespace LinkTracker.Bot.Application.Factories.ActionFactories;

public class TrackActionItemFactory : IActionItemFactory
{
    public string ProcessType => "Track";

    public ActionItem CreateInitialAction(Process process)
    {
        return new ActionItem
        {
            ProcessId = process.Id,
            ActionType = ActionType.AwaitingLink
        };
    }
}
