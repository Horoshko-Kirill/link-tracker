using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace LinkTracker.Tests.IntegrationTests.Fixtures
{
    public class TestEnvironment : IAsyncLifetime
    {
        private readonly INetwork _network;

        public IContainer Bot { get; private set; } = null!;
        public IContainer Scrapper { get; private set; } = null!;

        public string BotUrl => $"http://localhost:{Bot.GetMappedPublicPort(80)}";
        public string ScrapperUrl => $"http://localhost:{Scrapper.GetMappedPublicPort(80)}";

        public TestEnvironment()
        {
            Console.WriteLine("TEST ENV CREATED");

            _network = new NetworkBuilder()
                .WithName(Guid.NewGuid().ToString())
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _network.CreateAsync();

            Scrapper = new ContainerBuilder()
                .WithImage("link-tracker-scrapper:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("scrapper")
                .WithPortBinding(80, true)
                .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
                .Build();

            await Scrapper.StartAsync();

            Bot = new ContainerBuilder()
                .WithImage("link-tracker-bot:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("bot")
                .WithPortBinding(80, true)
                .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
                .WithEnvironment("Scrapper__BaseUrl", "http://scrapper:80")
                .WithEnvironment("UseFakeTelegramClient", "true")
                .WithEnvironment("Bot__Token", "Tests")
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilHttpRequestIsSucceeded(r => r
                            .ForPort(80)
                            .ForPath("/updates"))
                )
                .Build();

            await Bot.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await Bot.DisposeAsync();
            await Scrapper.DisposeAsync();
            await _network.DeleteAsync();
        }
    }
}
