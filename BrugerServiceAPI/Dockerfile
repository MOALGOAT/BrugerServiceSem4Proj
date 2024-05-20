FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

COPY BrugerServiceAPI/*.csproj BrugerServiceAPI/
WORKDIR /app/BrugerServiceAPI
RUN dotnet restore

COPY BrugerServiceAPI/. .
RUN dotnet publish -c Release -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/published-app .
ENTRYPOINT ["dotnet", "BrugerServiceAPI.dll"]
