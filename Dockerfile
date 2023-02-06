FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DotnetStarter.Api/DotnetStarter.Api.csproj", "DotnetStarter.Api/"]
RUN dotnet restore "DotnetStarter.Api/DotnetStarter.Api.csproj"
COPY [".config", "."]
RUN dotnet tool restore
COPY . .
WORKDIR "/src/DotnetStarter.Api"
RUN dotnet build "DotnetStarter.Api.csproj" -c Release

FROM build AS publish
RUN dotnet publish "DotnetStarter.Api.csproj" -c Release -o /app/publish
RUN dotnet ef migrations bundle --no-build -o /app/publish/migrations --configuration Release

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetStarter.Api.dll"]
