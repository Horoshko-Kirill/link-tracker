FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY src/LinkTracker.AiAgent/LinkTracker.AiAgent.csproj src/LinkTracker.AiAgent/
COPY src/LinkTracker.AiAgent.Application/LinkTracker.AiAgent.Application.csproj src/LinkTracker.AiAgent.Application/
COPY src/LinkTracker.AiAgent.Contracts/LinkTracker.AiAgent.Contracts.csproj src/LinkTracker.AiAgent.Contracts/
COPY src/LinkTracker.AiAgent.Infrastructure/LinkTracker.AiAgent.Infrastructure.csproj src/LinkTracker.AiAgent.Infrastructure/
COPY src/LinkTracker.AiAgent.Domain/LinkTracker.AiAgent.Domain.csproj src/LinkTracker.AiAgent.Domain/
COPY src/LinkTracker.Scrapper.Contracts/LinkTracker.Scrapper.Contracts.csproj src/LinkTracker.Scrapper.Contracts/
COPY src/LinkTracker.Bot.Contracts/LinkTracker.Bot.Contracts.csproj src/LinkTracker.Bot.Contracts/

RUN dotnet restore src/LinkTracker.AiAgent/LinkTracker.AiAgent.csproj

COPY src/LinkTracker.AiAgent/ src/LinkTracker.AiAgent/
COPY src/LinkTracker.AiAgent.Application/ src/LinkTracker.AiAgent.Application/
COPY src/LinkTracker.AiAgent.Contracts/ src/LinkTracker.AiAgent.Contracts/
COPY src/LinkTracker.AiAgent.Infrastructure/ src/LinkTracker.AiAgent.Infrastructure/
COPY src/LinkTracker.AiAgent.Domain/ src/LinkTracker.AiAgent.Domain/
COPY src/LinkTracker.Scrapper.Contracts/ src/LinkTracker.Scrapper.Contracts/
COPY src/LinkTracker.Bot.Contracts/ src/LinkTracker.Bot.Contracts/

RUN dotnet publish src/LinkTracker.AiAgent/LinkTracker.AiAgent.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libkrb5-3 \
        libgssapi-krb5-2 \
        krb5-user \
        && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LinkTracker.AiAgent.dll"]