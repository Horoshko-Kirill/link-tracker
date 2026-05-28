using System.Net;
using LinkTracker.Bot.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace LinkTracker.Bot.DI;

public static class KestrelExtensions
{
    public static IWebHostBuilder ConfigureKestrelWithProtocol(this IWebHostBuilder webHostBuilder)
    {
        return webHostBuilder.ConfigureKestrel((context, options) =>
        {
            if (context.HostingEnvironment.IsDevelopment())
            {
                return;
            }

            var kestrelOptions = context.Configuration
                .GetSection(KestrelOptions.SectionName)
                .Get<KestrelOptions>();

            var protocolType = kestrelOptions?.Type ?? "Grpc";
            var port = kestrelOptions?.Port ?? 80;
            var metricsPort = kestrelOptions?.MetricsPort ?? 9091;

            options.Listen(IPAddress.Any, port, listenOptions =>
            {
                listenOptions.Protocols = protocolType switch
                {
                    "Http" => HttpProtocols.Http1,
                    "Grpc" => HttpProtocols.Http2,
                    _ => HttpProtocols.Http2
                };
            });
            
            options.Listen(IPAddress.Any, metricsPort, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1;
            });
        });
    }
}