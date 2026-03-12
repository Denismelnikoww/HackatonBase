FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем solution и project файлы
COPY src/*.sln .
COPY src/Application/*.csproj Application/
COPY src/Domain/*.csproj Domain/
COPY src/Hackaton/*.csproj Hackaton/
COPY src/Infrastructure/*.csproj Infrastructure/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем все исходники
COPY src .

# Собираем и публикуем
WORKDIR /src/Hackaton
RUN dotnet build Web.csproj -c Release
RUN dotnet publish Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Устанавливаем curl для healthcheck (опционально)
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Копируем собранное приложение
COPY --from=build /app/publish .

# Создаем непривилегированного пользователя
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Настройки окружения
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "Web.dll"]