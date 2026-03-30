using LinkTracker.Bot.Clients.Scrapper;
using LinkTracker.Scrapper.Infrastructure.Clients;
using LinkTracker.Tests.IntegrationTests.Fixtures;

namespace LinkTracker.Tests.IntegrationTests.Clients;

public class TestClientsFactory
{
    public ScrapperClient Scrapper { get; }
    public BotClient Bot { get; }

    public TestClientsFactory(TestEnvironment env)
    {
        var scrapperHttp = new HttpClient
        {
            BaseAddress = new Uri(env.ScrapperUrl)
        };

        var botHttp = new HttpClient
        {
            BaseAddress = new Uri(env.BotUrl)
        };

        Scrapper = new ScrapperClient(scrapperHttp);
        Bot = new BotClient(botHttp);
    }
}
