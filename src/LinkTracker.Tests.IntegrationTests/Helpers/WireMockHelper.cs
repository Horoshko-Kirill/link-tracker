using System.Net.Http.Json;

namespace LinkTracker.Tests.IntegrationTests.Helpers;

public class WireMockHelper
{
    private readonly HttpClient _client;

    public WireMockHelper(string baseUrl)
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public async Task ResetAsync()
    {
        await _client.PostAsync("/__admin/reset", null);
    }

    public async Task StubAsync(
        string path,
        int statusCode,
        string body = "OK",
        int delayMs = 0)
    {
        var payload = new
        {
            request = new
            {
                method = "ANY",
                url = path
            },
            response = new
            {
                status = statusCode,
                body,
                fixedDelayMilliseconds = delayMs
            }
        };

        await _client.PostAsJsonAsync("/__admin/mappings", payload);
    }

    public async Task StubSequenceAsync(string path, params int[] statuses)
    {
        foreach (var status in statuses)
        {
            await StubAsync(path, status);
        }
    }
}