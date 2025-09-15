# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ChatGPTApiDemo.csproj
RUN dotnet publish ChatGPTApiDemo.csproj -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChatGPTApiDemo.dll"]
