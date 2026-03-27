using LinkTracker.Bot.Application.Handlers.ActionHandlers;
using LinkTracker.Bot.Application.Handlers.Interfaces;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Application.Services;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Bot;

public class ProcessOrchestratorTests
{
    [Fact]
    public async Task HandleMessageAsync_HandleCompleted()
    {
        var processRepository = Substitute.For<IProcessRepository>();
        var actionRepository = Substitute.For<IActionItemRepository>();
        var logger = Substitute.For<ILogger<ProcessOrchestrator>>();

        var handler = Substitute.For<IActionHandler>();
        handler.Action.Returns(ActionType.AwaitingLink.ToString());
        handler.HandleAsync(Arg.Any<Process>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
           .Returns(Task.CompletedTask);

        var handlers = new List<IActionHandler> { handler };

        var process = new Process
        {
            Id = 1
        };

        var actionItem = new ActionItem
        {
            ActionType = ActionType.AwaitingLink
        };

        long chatId = 123;

        processRepository.GetActiveProcessAsync(chatId, Arg.Any<CancellationToken>()).Returns(process);
        actionRepository.GetAllAsync(process.Id, Arg.Any<CancellationToken>()).Returns(new List<ActionItem> { actionItem });
        actionRepository.GetLastAsync(process.Id, Arg.Any<CancellationToken>()).Returns(actionItem);

        var orchestrator = new ProcessOrchestrator(handlers, processRepository, actionRepository, logger);

        await orchestrator.HandleMessageAsync(chatId, string.Empty);

        await handler.Received(1).HandleAsync(process, string.Empty, Arg.Any<CancellationToken>());
    }
}
