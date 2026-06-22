using System.Net;
using System.Net.Http.Json;
using LinkTracker.Tests.IntegrationTests.Fixtures;

namespace LinkTracker.Tests.IntegrationTests.Valkey;

[Collection("Integration")]
public class ValkeyCacheTests
{
    private readonly TestEnvironment _env;

    public ValkeyCacheTests(TestEnvironment env)
    {
        _env = env;
    }

    [Fact]
    public async Task List_Should_Save_Response_To_Valkey_Cache()
    {
        var chatId = 5035572703L;
        var key = $"links:{{{chatId}}}:all";

        using var client = new HttpClient
        {
            BaseAddress = new Uri(_env.ScrapperUrl)
        };

        var registerResponse = await client.PostAsync($"/tg-chat/{chatId}", null);
        Assert.True(
            registerResponse.StatusCode is HttpStatusCode.OK or HttpStatusCode.Conflict,
            await registerResponse.Content.ReadAsStringAsync());

        var request = new HttpRequestMessage(HttpMethod.Get, "/links");
        request.Headers.Add("Tg-Chat-Id", chatId.ToString());

        var listResponse = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);

        var cacheResult = await _env.Valkey.ExecAsync([
            "valkey-cli",
            "GET",
            key
        ]);

        Assert.Equal(0, cacheResult.ExitCode);
        Assert.Contains("\"size\"", cacheResult.Stdout, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddLink_Should_Invalidate_List_Cache()
    {
        var chatId = 5035572704L;
        var key = $"links:{{{chatId}}}:all";

        using var client = new HttpClient
        {
            BaseAddress = new Uri(_env.ScrapperUrl)
        };

        var registerResponse = await client.PostAsync($"/tg-chat/{chatId}", null);
        Assert.True(
            registerResponse.StatusCode is HttpStatusCode.OK or HttpStatusCode.Conflict,
            await registerResponse.Content.ReadAsStringAsync());

        var listRequest = new HttpRequestMessage(HttpMethod.Get, "/links");
        listRequest.Headers.Add("Tg-Chat-Id", chatId.ToString());

        var firstListResponse = await client.SendAsync(listRequest);
        Assert.Equal(HttpStatusCode.OK, firstListResponse.StatusCode);

        var cachedBeforeAdd = await _env.Valkey.ExecAsync([
            "valkey-cli",
            "EXISTS",
            key
        ]);

        Assert.Contains("1", cachedBeforeAdd.Stdout);

        var addRequest = new HttpRequestMessage(HttpMethod.Post, "/links");
        addRequest.Headers.Add("Tg-Chat-Id", chatId.ToString());
        addRequest.Content = JsonContent.Create(new
        {
            url = "https://github.com/test/repo",
            tags = Array.Empty<string>(),
            filters = Array.Empty<string>()
        });

        var addResponse = await client.SendAsync(addRequest);
        Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);

        var cachedAfterAdd = await _env.Valkey.ExecAsync([
            "valkey-cli",
            "EXISTS",
            key
        ]);

        Assert.Contains("0", cachedAfterAdd.Stdout);
    }
}