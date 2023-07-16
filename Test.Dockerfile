FROM mcr.microsoft.com/dotnet/sdk:8.0-preview-jammy AS build
WORKDIR /src
COPY ["DotnetStarter.Test/DotnetStarter.Test.csproj", "DotnetStarter.Test/"]
RUN dotnet restore "DotnetStarter.Test/DotnetStarter.Test.csproj" -r linux-x64
COPY . .
WORKDIR "/src/DotnetStarter.Test"
RUN dotnet build "DotnetStarter.Test.csproj" -c Debug

# Install PowerShell
RUN apt-get install -y wget apt-transport-https
RUN wget -q https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get install -y powershell

FROM build AS final
ENV SNAPSHOOTER_STRICT_MODE="true"
WORKDIR "/src/DotnetStarter.Test"
ENTRYPOINT ["dotnet", "test", "--no-build"]
