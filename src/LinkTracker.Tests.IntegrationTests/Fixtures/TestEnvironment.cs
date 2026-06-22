using Confluent.Kafka;
using Confluent.Kafka.Admin;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace LinkTracker.Tests.IntegrationTests.Fixtures;

public class TestEnvironment : IAsyncLifetime
{
    private readonly INetwork _network;

    public IContainer Bot { get; private set; } = null!;
    public IContainer Scrapper { get; private set; } = null!;
    public IContainer Db { get; private set; } = null!;
    public IContainer BotMigrator { get; private set; } = null!;
    public IContainer ScrapperMigrator { get; private set; } = null!;
    public IContainer Kafka { get; private set; } = null!;
    public IContainer Zookeeper { get; private set; } = null!;
    public IContainer Valkey { get; private set; } = null!;
    public IContainer WireMock { get; private set; } = null!;
    public IContainer AiAgent { get; private set; } = null!;
    private static string DockerHost =>
        Environment.GetEnvironmentVariable("TESTCONTAINERS_HOST_OVERRIDE") ?? "localhost";

    public string BotUrl => $"http://{DockerHost}:{Bot.GetMappedPublicPort(80)}";
    public string ScrapperUrl => $"http://{DockerHost}:{Scrapper.GetMappedPublicPort(80)}";
    public string KafkaBootstrapAddress => $"{DockerHost}:{Kafka.GetMappedPublicPort(9092)}";
    public string WireMockUrl => $"http://{DockerHost}:{WireMock.GetMappedPublicPort(8080)}";
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

        Zookeeper = new ContainerBuilder()
            .WithImage("confluentinc/cp-zookeeper:7.6.1")
            .WithNetwork(_network)
            .WithNetworkAliases("zookeeper")
            .WithEnvironment("ZOOKEEPER_CLIENT_PORT", "2181")
            .WithEnvironment("ZOOKEEPER_TICK_TIME", "2000")
            .Build();

        await Zookeeper.StartAsync();

        Kafka = new ContainerBuilder()
            .WithImage("confluentinc/cp-kafka:7.6.1")
            .WithNetwork(_network)
            .WithNetworkAliases("kafka")
            .WithPortBinding(9092, true)
            .WithEnvironment("KAFKA_BROKER_ID", "1")
            .WithEnvironment("KAFKA_ZOOKEEPER_CONNECT", "zookeeper:2181")
            .WithEnvironment("KAFKA_LISTENERS", "INTERNAL://0.0.0.0:29092,EXTERNAL://0.0.0.0:9092")
            .WithEnvironment("KAFKA_ADVERTISED_LISTENERS", "INTERNAL://kafka:29092,EXTERNAL://localhost:9092")
            .WithEnvironment("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "INTERNAL:PLAINTEXT,EXTERNAL:PLAINTEXT")
            .WithEnvironment("KAFKA_INTER_BROKER_LISTENER_NAME", "INTERNAL")
            .WithEnvironment("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR", "1")
            .WithEnvironment("KAFKA_TRANSACTION_STATE_LOG_MIN_ISR", "1")
            .Build();

        await Kafka.StartAsync();

        await Kafka.ExecAsync([
            "bash",
            "-c",
            "cub kafka-ready -b kafka:29092 1 60 && " +
            "kafka-topics --bootstrap-server kafka:29092 --create --if-not-exists --topic link-updates --partitions 3 --replication-factor 1 &&" +
            "kafka-topics --bootstrap-server kafka:29092 --create --if-not-exists --topic link.raw-updates --partitions 1 --replication-factor 1 && " +
            "kafka-topics --bootstrap-server kafka:29092 --create --if-not-exists --topic link.processed-updates --partitions 1 --replication-factor 1"
        ]);

        Valkey = new ContainerBuilder()
            .WithImage("valkey/valkey:8.0")
            .WithNetwork(_network)
            .WithNetworkAliases("valkey")
            .WithCommand("valkey-server", "--protected-mode", "no")
            .WithPortBinding(6379, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilCommandIsCompleted("valkey-cli ping"))
            .Build();

        await Valkey.StartAsync();

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

        WireMock = new ContainerBuilder()
            .WithImage("wiremock/wiremock:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("wiremock")
            .WithPortBinding(8080, true)
            .WithCommand("--global-response-templating", "--verbose")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8080).ForPath("/__admin")))
            .Build();

        await WireMock.StartAsync();

        Scrapper = new ContainerBuilder()
            .WithImage("link-tracker-scrapper:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("scrapper")
            .WithPortBinding(80, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
            .WithEnvironment("TelegramBot__BaseUrl", "http://wiremock:8080")
            .WithEnvironment("Database__ConnectionString",
                "Host=linktracker.db;Port=5432;Database=linktracker_test;Username=postgres;Password=postgres")
            .WithEnvironment("KESTREL__PORT", "80")
            .WithEnvironment("ClientType__Type", "Http")
            .WithEnvironment("KESTREL__Type", "Http")
            .WithEnvironment("NotificationTransport", "Kafka")
            .WithEnvironment("Kafka__BootstrapServers", "kafka:29092")
            .WithEnvironment("Kafka__Topic", "link-updates")
            .WithEnvironment("Valkey__Configuration", "valkey:6379,abortConnect=false,connectRetry=5,connectTimeout=10000,syncTimeout=10000,keepAlive=10,allowAdmin=true,ssl=false")
            .WithEnvironment("Valkey__DefaultTtlMinutes", "00:10:00")
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
            .WithEnvironment("KESTREL__PORT", "80")
            .WithEnvironment("ClientType__Type", "Http")
            .WithEnvironment("KESTREL__Type", "Http")
            .WithEnvironment("Kafka__BootstrapServers", "kafka:29092")
            .WithEnvironment("Kafka__Topic", "link-updates")
            .WithEnvironment("Kafka__GroupId", "link-tracker-bot-tests")
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r
                        .ForPort(80)
                        .ForPath("/updates")))
            .Build();

        await Bot.StartAsync();

        AiAgent = new ContainerBuilder()
            .WithImage("link-tracker-ai-agent:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("ai-agent")
            .WithPortBinding(80, true)
            .WithEnvironment("ASPNETCORE_URLS", "http://+:80")
            .WithEnvironment("Kafka__BootstrapServers", "kafka:29092")
            .WithEnvironment("Kafka__ConsumerTopic", "link.raw-updates")
            .WithEnvironment("Kafka__ProduceTopic", "link.processed-updates")
            .WithEnvironment("Kafka__GroupId", "ai-agent-tests")
            .WithEnvironment("AiAgent__Summarization__ApiKey", "test")
            .Build();

        await AiAgent.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Bot.DisposeAsync();
        await Scrapper.DisposeAsync();
        await ScrapperMigrator.DisposeAsync();
        await BotMigrator.DisposeAsync();
        await Kafka.DisposeAsync();
        await Zookeeper.DisposeAsync();
        await Db.DisposeAsync();
        await Valkey.DisposeAsync();
        await WireMock.DisposeAsync();
        await AiAgent.DisposeAsync();
        await _network.DeleteAsync();
    }
}