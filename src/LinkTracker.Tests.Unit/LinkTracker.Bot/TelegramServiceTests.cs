using LinkTracker.Bot.Application.Commands.Interfaces;
using LinkTracker.Bot.Application.InterfacesClients;
using LinkTracker.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LinkTracker.Tests.Unit.LinkTracker.Bot;

public class TelegramServiceTests
{
    /// <summary>
    /// Положительный тест на установку команд телеграмму 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task StartAsync_ShouldSetCommands()
    {
        var client = Substitute.For<ITelegramClient>();

        var start = Substitute.For<ICommand>();
        start.Name.Returns("/start");

        var commands = new List<ICommand> { start };

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider
            .GetService(typeof(IEnumerable<ICommand>))
            .Returns(commands);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(serviceProvider);

        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);

        var service = new TelegramHostedService(client, scopeFactory);

        await service.StartAsync(CancellationToken.None);

        await client.Received(1)
            .SetCommandsAsync(commands, Arg.Any<CancellationToken>());
    }
}
