FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TimeTracker.CommandLine/TimeTracker.CommandLine.csproj", "TimeTracker.CommandLine/"]
RUN dotnet restore "TimeTracker.CommandLine/TimeTracker.CommandLine.csproj"
COPY . .
WORKDIR "/src/TimeTracker.CommandLine"
RUN dotnet build "TimeTracker.CommandLine.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TimeTracker.CommandLine.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeTracker.CommandLine.dll"]
