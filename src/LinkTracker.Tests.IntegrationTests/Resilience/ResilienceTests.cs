using System.Diagnostics;
using System.Net.Http.Json;
using LinkTracker.Tests.IntegrationTests.Fixtures;
using LinkTracker.Tests.IntegrationTests.Helpers;

namespace LinkTracker.Tests.IntegrationTests.Resilience;

[Collection("Integration")]
public class ResilienceTests
{
    private readonly TestEnvironment _env;
    private readonly WireMockHelper _wiremock;

    public ResilienceTests(TestEnvironment env)
    {
        _env = env;
        _wiremock = new WireMockHelper(env.WireMockUrl);
    }

    [Fact]
    public async Task TimeoutExceeded_ReturnsFastFailure()
    {
        await _wiremock.ResetAsync();

        await _wiremock.StubAsync(
            "/updates",
            200,
            "OK",
            delayMs: 10000);

        using var client = new HttpClient
        {
            BaseAddress = new Uri(_env.ScrapperUrl)
        };

        var stopwatch = Stopwatch.StartNew();

        var response = await client.PostAsJsonAsync("/some-trigger-endpoint", new
        {
            value = "test"
        });

        stopwatch.Stop();

        Assert.False(response.IsSuccessStatusCode);

        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(10),
            "Timeout policy did not stop request early");
    }
    
    [Fact]
    public async Task CircuitBreaker_Should_Open()
    {
        await _wiremock.ResetAsync();

        await _wiremock.StubAsync("/updates", 500);

        using var client = new HttpClient
        {
            BaseAddress = new Uri(_env.ScrapperUrl)
        };

        for (int i = 0; i < 10; i++)
        {
            await client.PostAsJsonAsync("/some-trigger-endpoint", new {});
        }

        var sw = Stopwatch.StartNew();

        var response = await client.PostAsJsonAsync("/some-trigger-endpoint", new {});

        sw.Stop();

        Assert.True(sw.Elapsed < TimeSpan.FromMilliseconds(500));
    }
    
}