# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props nuget.config global.json Pricing.sln ./
COPY src/ src/
COPY tests/ tests/
RUN dotnet restore Pricing.sln
RUN dotnet build Pricing.sln -c Release --no-restore

FROM build AS test
RUN dotnet test Pricing.sln -c Release --no-build --logger "console;verbosity=normal"

FROM build AS pack
ARG VERSION=1.0.0
RUN dotnet pack src/Pricing.Client/Pricing.Client.csproj -c Release --no-restore -p:Version=${VERSION} -o /packages

FROM scratch AS packages
COPY --from=pack /packages /

FROM build AS publish
RUN dotnet publish src/Pricing.Api/Pricing.Api.csproj -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pricing.Api.dll"]
