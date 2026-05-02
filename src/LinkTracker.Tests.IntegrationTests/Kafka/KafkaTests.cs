using System.Text.Json;
using LinkTracker.Bot.Contracts.Dto;
using LinkTracker.Tests.IntegrationTests.Fixtures;

namespace LinkTracker.Tests.IntegrationTests.Kafka;

[Collection("Integration")]
public class KafkaTests
{
    private readonly TestEnvironment _env;

    public KafkaTests(TestEnvironment env)
    {
        _env = env;
    }

    [Fact]
    public async Task Bot_Should_Consume_LinkUpdate_From_Kafka()
    {
        var topic = "link-updates";
        var groupId = "link-tracker-bot-tests";

        var linkUpdate = new LinkUpdate
        {
            Url = "https://github.com/test/repo",
            Description = "Kafka integration test message",
            ChatIds = [123456]
        };

        var json = JsonSerializer.Serialize(linkUpdate);
        var escapedJson = EscapeForBashSingleQuotes(json);
        var escapedKey = EscapeForBashSingleQuotes(linkUpdate.Url);

        var produceResult = await _env.Kafka.ExecAsync([
            "bash",
            "-c",
            $"echo '{escapedKey}|{escapedJson}' | " +
            $"kafka-console-producer " +
            $"--bootstrap-server kafka:29092 " +
            $"--topic {topic} " +
            $"--property parse.key=true " +
            $"--property key.separator='|'"
        ]);

        Assert.Equal(0, produceResult.ExitCode);

        await WaitUntilAsync(
            async () => await ConsumerGroupHasCommittedOffsetAsync(groupId, topic),
            TimeSpan.FromSeconds(90));
    }

    private async Task<bool> ConsumerGroupHasCommittedOffsetAsync(string groupId, string topic)
    {
        var result = await _env.Kafka.ExecAsync([
            "bash",
            "-c",
            $"kafka-consumer-groups " +
            $"--bootstrap-server kafka:29092 " +
            $"--group {groupId} " +
            $"--describe || true"
        ]);

        if (result.ExitCode != 0)
        {
            return false;
        }

        var lines = result.Stdout
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var line in lines)
        {
            if (!line.Contains(topic))
            {
                continue;
            }

            var columns = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (columns.Any(x => long.TryParse(x, out var value) && value > 0))
            {
                return true;
            }
        }

        return false;
    }

    private async Task WaitUntilAsync(
        Func<Task<bool>> condition,
        TimeSpan timeout)
    {
        var startedAt = DateTimeOffset.UtcNow;

        while (DateTimeOffset.UtcNow - startedAt < timeout)
        {
            if (await condition())
            {
                return;
            }

            await Task.Delay(500);
        }

        var botLogs = await _env.Bot.GetLogsAsync();
        var kafkaLogs = await _env.Kafka.GetLogsAsync();

        throw new TimeoutException(
            "Condition was not satisfied in time." +
            Environment.NewLine +
            "BOT STDOUT:" +
            Environment.NewLine +
            botLogs.Stdout +
            Environment.NewLine +
            "BOT STDERR:" +
            Environment.NewLine +
            botLogs.Stderr +
            Environment.NewLine +
            "KAFKA STDOUT:" +
            Environment.NewLine +
            kafkaLogs.Stdout +
            Environment.NewLine +
            "KAFKA STDERR:" +
            Environment.NewLine +
            kafkaLogs.Stderr);
    }

    private static string EscapeForBashSingleQuotes(string value)
    {
        return value.Replace("'", "'\"'\"'");
    }
}