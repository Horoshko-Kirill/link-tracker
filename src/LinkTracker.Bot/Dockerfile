FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY src/LinkTracker.Bot/LinkTracker.Bot.csproj src/LinkTracker.Bot/
COPY src/LinkTracker.Bot.Application/LinkTracker.Bot.Application.csproj src/LinkTracker.Bot.Application/
COPY src/LinkTracker.Bot.Contracts/LinkTracker.Bot.Contracts.csproj src/LinkTracker.Bot.Contracts/
COPY src/LinkTracker.Bot.Infrastructure/LinkTracker.Bot.Infrastructure.csproj src/LinkTracker.Bot.Infrastructure/
COPY src/LinkTracker.Bot.Domain/LinkTracker.Bot.Domain.csproj src/LinkTracker.Bot.Domain/
COPY src/LinkTracker.Scrapper.Contracts/LinkTracker.Scrapper.Contracts.csproj src/LinkTracker.Scrapper.Contracts/

RUN dotnet restore src/LinkTracker.Bot/LinkTracker.Bot.csproj

COPY src/LinkTracker.Bot/ src/LinkTracker.Bot/
COPY src/LinkTracker.Bot.Application/ src/LinkTracker.Bot.Application/
COPY src/LinkTracker.Bot.Contracts/ src/LinkTracker.Bot.Contracts/
COPY src/LinkTracker.Bot.Infrastructure/ src/LinkTracker.Bot.Infrastructure/
COPY src/LinkTracker.Bot.Domain/ src/LinkTracker.Bot.Domain/
COPY src/LinkTracker.Scrapper.Contracts/ src/LinkTracker.Scrapper.Contracts/

RUN dotnet publish src/LinkTracker.Bot/LinkTracker.Bot.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libkrb5-3 \
        libgssapi-krb5-2 \
        krb5-user \
        && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LinkTracker.Bot.dll"]