using System.Text.Json;
using DotNet.Testcontainers.Containers;
using LinkTracker.AiAgent.Domain.Models;
using LinkTracker.Tests.IntegrationTests.Fixtures;

namespace LinkTracker.Tests.IntegrationTests.AiAgent;

[Collection("Integration")]
public class AiAgentTests
{
    private readonly TestEnvironment _env;

    public AiAgentTests(TestEnvironment env)
    {
        _env = env;
    }

    [Fact]
    public async Task AiAgent_Should_Consume_And_Process_Valid_Message()
    {
        await Task.Delay(3000);
        
        var rawTopic = "link.raw-updates";
        var processedTopic = "link.processed-updates";

        var update = new RawLinkUpdate
        {
            EventId = "1",
            Url = "https://github.com/test/repo",
            Description = """
                          Very long update text
                          that should be processed
                          by AI agent
                          """,
            ChatIds = new List<long> { 1 }
        };

        var json = JsonSerializer.Serialize(update);

        var produceResult = await _env.Kafka.ExecAsync([
            "bash",
            "-c",
            $"echo '{Escape(json)}' | kafka-console-producer " +
            $"--bootstrap-server kafka:29092 " +
            $"--topic {rawTopic}"
        ]);

        Assert.Equal(0, produceResult.ExitCode);

        await WaitUntilAsync(
            async () => await TopicContainsMessages(processedTopic),
            TimeSpan.FromSeconds(60));
    }

    [Fact]
    public async Task AiAgent_Should_Not_Crash_On_Invalid_Message()
    {
        var rawTopic = "link.raw-updates";

        var invalidJson = "{ invalid json }";

        var produceResult = await _env.Kafka.ExecAsync([
            "bash",
            "-c",
            $"echo '{Escape(invalidJson)}' | kafka-console-producer " +
            $"--bootstrap-server kafka:29092 " +
            $"--topic {rawTopic}"
        ]);

        Assert.Equal(0, produceResult.ExitCode);

        await Task.Delay(5000);

        var logs = await _env.AiAgent.GetLogsAsync();

        Assert.Contains(
            "deserialization",
            (logs.Stdout + logs.Stderr).ToLower());

        Assert.True(_env.AiAgent.State == TestcontainersStates.Running);
    }

    private async Task<bool> TopicContainsMessages(string topic)
    {
        var result = await _env.Kafka.ExecAsync([
            $"kafka-console-consumer --bootstrap-server kafka:29092 --topic {topic} --from-beginning --max-messages 1 --timeout-ms 5000"
        ]);

        var output = result.Stdout ?? "";

        return output.Contains("github") 
               || output.Contains("test") 
               || output.Length > 0;
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

        throw new TimeoutException("Condition was not satisfied in time.");
    }

    private static string Escape(string value)
    {
        return value.Replace("'", "'\"'\"'");
    }
}