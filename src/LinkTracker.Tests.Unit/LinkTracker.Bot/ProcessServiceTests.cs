using LinkTracker.Bot.Application.Exceptions;
using LinkTracker.Bot.Application.Factories.Interfaces;
using LinkTracker.Bot.Application.InterfacesRepositories;
using LinkTracker.Bot.Application.Services;
using LinkTracker.Bot.Domain.Enums;
using LinkTracker.Bot.Domain.Models;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Bot;

public class ProcessServiceTests
{
    [Fact]
    public async Task StartProcessAsync_ShouldAddAction()
    {
        long chatId = 123;
        string processType = "Track";

        var processRepository = Substitute.For<IProcessRepository>();
        var actionItemRepository = Substitute.For<IActionItemRepository>();
        var actionItemFactory = Substitute.For<IActionItemFactory>();

        var process = new Process
        {
            Id = 1,
            ChatId = chatId,
            Status = ProcessStatus.Active
        };

        var actionItem = new ActionItem
        {
            ProcessId = process.Id
        };

        processRepository.GetActiveProcessAsync(chatId, Arg.Any<CancellationToken>())
            .Returns((Process?)null, process);

        actionItemFactory.ProcessType.Returns(processType);
        actionItemFactory.CreateInitialAction(process).Returns(actionItem);

        var actionItemFactories = new List<IActionItemFactory> { actionItemFactory };

        var processService = new ProcessService(processRepository, actionItemRepository, actionItemFactories);

        await processService.StartProcessAsync(chatId, processType);

        await processRepository.Received(1).CreateAsync(Arg.Any<Process>(), Arg.Any<CancellationToken>());

        await processRepository.Received(2).GetActiveProcessAsync(chatId, Arg.Any<CancellationToken>());

        await actionItemRepository.Received(1).AddAsync(actionItem, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task StartProcessAsync_ExistingProcess()
    {
        var chatId = 123;
        string processType = "Track";

        var processRepository = Substitute.For<IProcessRepository>();
        var actionItemRepository = Substitute.For<IActionItemRepository>();
        var actionItemFactories = Substitute.For<IEnumerable<IActionItemFactory>>();

        var existProcess = new Process
        {
            Id = 1,
            ChatId = chatId,
            Status = ProcessStatus.Active
        };

        processRepository.GetActiveProcessAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(existProcess);

        var processService = new ProcessService(processRepository, actionItemRepository, actionItemFactories);

        await Assert.ThrowsAsync<ProcessAlreadyExistsException>(() => processService.StartProcessAsync(chatId, processType));
    }

    [Fact]
    public async Task StartProcessAsync_ShouldCreateException()
    {
        long chatId = 123;
        string processType = "Track";

        var processRepository = Substitute.For<IProcessRepository>();
        var actionItemRepository = Substitute.For<IActionItemRepository>();
        var actionItemFactories = Substitute.For<IEnumerable<IActionItemFactory>>();

        var process = new Process
        {
            Id = 1,
            ChatId = chatId,
            Status = ProcessStatus.Active
        };

        processRepository.GetActiveProcessAsync(chatId, Arg.Any<CancellationToken>())
            .Returns((Process?)null, (Process?)null);

        var processService = new ProcessService(processRepository, actionItemRepository, actionItemFactories);

        await Assert.ThrowsAsync<ProcessNotFoundException>(() => processService.StartProcessAsync(chatId, processType));
    }
}
