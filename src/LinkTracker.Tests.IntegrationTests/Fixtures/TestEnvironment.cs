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
        public IContainer Db { get; private set; } = null!;
        public IContainer BotMigrator { get; private set; } = null!;
        public IContainer ScrapperMigrator { get; private set; } = null!;

        public string BotUrl => $"http://localhost:{Bot.GetMappedPublicPort(80)}";
        public string ScrapperUrl => $"http://localhost:{Scrapper.GetMappedPublicPort(80)}";

        public TestEnvironment()
        {
            _network = new NetworkBuilder()
                .WithName(Guid.NewGuid().ToString())
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _network.CreateAsync();

            Db = new ContainerBuilder()
                .WithImage("postgres:16")
                .WithNetwork(_network)
                .WithNetworkAliases("linktracker.db")
                .WithEnvironment("POSTGRES_DB", "linktracker_test")
                .WithEnvironment("POSTGRES_USER", "postgres")
                .WithEnvironment("POSTGRES_PASSWORD", "postgres")
                .WithPortBinding(5432, true)
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilCommandIsCompleted("pg_isready -U postgres"))
                .Build();

            await Db.StartAsync();

            BotMigrator = new ContainerBuilder()
                .WithImage("link-tracker-bot.migrator:latest")
                .WithNetwork(_network)
                .WithEnvironment("Database__ConnectionString",
                    "Host=linktracker.db;Port=5432;Database=linktracker_test;Username=postgres;Password=postgres")
                .Build();

            await BotMigrator.StartAsync();
            await BotMigrator.GetExitCodeAsync();

            ScrapperMigrator = new ContainerBuilder()
                .WithImage("link-tracker-scrapper.migrator:latest")
                .WithNetwork(_network)
                .WithEnvironment("Database__ConnectionString",
                    "Host=linktracker.db;Port=5432;Database=linktracker_test;Username=postgres;Password=postgres")
                .Build();

            await ScrapperMigrator.StartAsync();
            await ScrapperMigrator.GetExitCodeAsync();

            Scrapper = new ContainerBuilder()
                .WithImage("link-tracker-scrapper:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("scrapper")
                .WithPortBinding(80, true)
                .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
                .WithEnvironment("Database__ConnectionString",
                    "Host=linktracker.db;Port=5432;Database=linktracker_test;Username=postgres;Password=postgres")
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
                .WithEnvironment("Database__ConnectionString",
                    "Host=linktracker.db;Port=5432;Database=linktracker_test;Username=postgres;Password=postgres")
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilHttpRequestIsSucceeded(r => r
                            .ForPort(80)
                            .ForPath("/updates")))
                .Build();

            await Bot.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await Bot.DisposeAsync();
            await Scrapper.DisposeAsync();
            await ScrapperMigrator.DisposeAsync();
            await BotMigrator.DisposeAsync();
            await Db.DisposeAsync();
            await _network.DeleteAsync();
        }
    }
}