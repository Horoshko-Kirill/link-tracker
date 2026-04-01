FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/LinkTracker.Bot.Migrator/LinkTracker.Bot.Migrator.csproj src/LinkTracker.Bot.Migrator/
RUN dotnet restore src/LinkTracker.Bot.Migrator/LinkTracker.Bot.Migrator.csproj

COPY src/LinkTracker.Bot.Migrator/ src/LinkTracker.Bot.Migrator/
RUN dotnet publish src/LinkTracker.Bot.Migrator/LinkTracker.Bot.Migrator.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        libkrb5-3 \
        libgssapi-krb5-2 \
        krb5-user \
        && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /app/publish .
COPY migrations /app/migrations/

ENTRYPOINT ["dotnet", "LinkTracker.Bot.Migrator.dll"]