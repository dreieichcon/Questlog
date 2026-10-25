FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Questlog.Core/Questlog.Core.csproj", "Questlog.Core/"]
COPY ["Questlog.Repositories/Questlog.Repositories.csproj", "Questlog.Repositories/"]
COPY ["Questlog.Services/Questlog.Services.csproj", "Questlog.Services/"]
COPY ["Questlog.Ui/Questlog.Ui.csproj", "Questlog.Ui/"]
COPY ["Questlog/Questlog.csproj", "Questlog/"]

RUN dotnet restore "Questlog/Questlog.csproj"
COPY . .
WORKDIR "/src/Questlog"
RUN dotnet build "./Questlog.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Questlog.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Questlog.dll"]
