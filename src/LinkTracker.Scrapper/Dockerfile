FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY src/LinkTracker.Scrapper/LinkTracker.Scrapper.csproj src/LinkTracker.Scrapper/
COPY src/LinkTracker.Scrapper.Application/LinkTracker.Scrapper.Application.csproj src/LinkTracker.Scrapper.Application/
COPY src/LinkTracker.Scrapper.Contracts/LinkTracker.Scrapper.Contracts.csproj src/LinkTracker.Scrapper.Contracts/
COPY src/LinkTracker.Scrapper.Infrastructure/LinkTracker.Scrapper.Infrastructure.csproj src/LinkTracker.Scrapper.Infrastructure/
COPY src/LinkTracker.Scrapper.Domain/LinkTracker.Scrapper.Domain.csproj src/LinkTracker.Scrapper.Domain/
COPY src/LinkTracker.Bot.Contracts/LinkTracker.Bot.Contracts.csproj src/LinkTracker.Bot.Contracts/

RUN dotnet restore src/LinkTracker.Scrapper/LinkTracker.Scrapper.csproj

COPY src/LinkTracker.Scrapper/ src/LinkTracker.Scrapper/
COPY src/LinkTracker.Scrapper.Application/ src/LinkTracker.Scrapper.Application/
COPY src/LinkTracker.Scrapper.Contracts/ src/LinkTracker.Scrapper.Contracts/
COPY src/LinkTracker.Scrapper.Infrastructure/ src/LinkTracker.Scrapper.Infrastructure/
COPY src/LinkTracker.Scrapper.Domain/ src/LinkTracker.Scrapper.Domain/
COPY src/LinkTracker.Bot.Contracts/ src/LinkTracker.Bot.Contracts/

RUN dotnet publish src/LinkTracker.Scrapper/LinkTracker.Scrapper.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libkrb5-3 \
        libgssapi-krb5-2 \
        krb5-user \
        && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LinkTracker.Scrapper.dll"]