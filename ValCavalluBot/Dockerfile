#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ValCavalluBot/ValCavalluBot.csproj", "ValCavalluBot/"]
COPY ["AutoUpdater/AutoUpdaterClient.csproj", "AutoUpdater/"]
COPY ["GRPCClient/GRPCClient.csproj", "GRPCClient/"]
COPY ["Shares/Shares.csproj", "Shares/"]
COPY ["HowrseBotClient/HowrseBotClient.csproj", "HowrseBotClient/"]
RUN dotnet restore "ValCavalluBot/ValCavalluBot.csproj"
COPY . .
WORKDIR "/src/ValCavalluBot"
RUN dotnet build "ValCavalluBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ValCavalluBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ValCavalluBot.dll"]