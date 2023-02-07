FROM mcr.microsoft.com/dotnet/aspnet:7.0-jammy AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy AS build
WORKDIR /src
COPY ["DotnetStarter.Api/DotnetStarter.Api.csproj", "DotnetStarter.Api/"]
RUN dotnet restore "DotnetStarter.Api/DotnetStarter.Api.csproj" -r linux-x64
COPY [".config", "."]
RUN dotnet tool restore
COPY . .
WORKDIR "/src/DotnetStarter.Api"
RUN dotnet build "DotnetStarter.Api.csproj" -c Release

FROM build AS publish
RUN dotnet publish "DotnetStarter.Api.csproj" -c Release -o /app/publish
RUN dotnet ef migrations bundle --verbose --no-build -o /app/publish/migrations --configuration Release

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotnetStarter.Api.dll"]
