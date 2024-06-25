# Stage 1: Базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Stage 2: Сборка приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["./CommandsService.csproj", "./"]
RUN dotnet restore "./CommandsService.csproj"

# Копируем все остальные файлы и выполняем сборку
COPY . .
WORKDIR "/src"
RUN dotnet build "./CommandsService.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# Stage 3: Публикация приложения
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommandsService.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# Stage 4: Финальный образ для запуска приложения
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommandsService.dll"]
