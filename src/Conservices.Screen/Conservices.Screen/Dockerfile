FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

ENV TZ=Europe/Berlin

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Conservices.Screen.Models/Conservices.Screen.Models.csproj", "Conservices.Screen.Models/"]
COPY ["Conservices.Screen.Util/Conservices.Screen.Util.csproj", "Conservices.Screen.Util/"]
COPY ["Conservices.Screen.Interfaces/Conservices.Screen.Interfaces.csproj", "Conservices.Screen.Interfaces/"]
COPY ["Conservices.Screen.Repositories/Conservices.Screen.Repositories.csproj", "Conservices.Screen.Repositories/"]
COPY ["Conservices.Screen.Services/Conservices.Screen.Services.csproj", "Conservices.Screen.Services/"]
COPY ["Conservices.Screen.Ui/Conservices.Screen.Ui.csproj", "Conservices.Screen.Ui/"]
COPY ["Conservices.Screen/Conservices.Screen.csproj", "Conservices.Screen/"]

RUN dotnet restore "Conservices.Screen/Conservices.Screen.csproj"
COPY . .
WORKDIR "/src/Conservices.Screen"
RUN dotnet build "./Conservices.Screen.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Conservices.Screen.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Conservices.Screen.dll"]