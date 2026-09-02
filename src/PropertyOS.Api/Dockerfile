# src/PropertyOS.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution-level files first so restore is cached across builds
COPY Directory.Build.props Directory.Packages.props PropertyOS.sln ./
COPY src/PropertyOS.Domain/PropertyOS.Domain.csproj src/PropertyOS.Domain/
COPY src/PropertyOS.Application/PropertyOS.Application.csproj src/PropertyOS.Application/
COPY src/PropertyOS.Infrastructure/PropertyOS.Infrastructure.csproj src/PropertyOS.Infrastructure/
COPY src/PropertyOS.Contracts/PropertyOS.Contracts.csproj src/PropertyOS.Contracts/
COPY src/PropertyOS.Api/PropertyOS.Api.csproj src/PropertyOS.Api/

RUN dotnet restore src/PropertyOS.Api/PropertyOS.Api.csproj

COPY src/ src/
WORKDIR /src/src/PropertyOS.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY src/PropertyOS.Api/docker-entrypoint.sh /app/docker-entrypoint.sh

ENV ASPNETCORE_ENVIRONMENT=Staging
EXPOSE 8080

RUN chmod +x /app/docker-entrypoint.sh
ENTRYPOINT ["/app/docker-entrypoint.sh"]