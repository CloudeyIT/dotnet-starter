FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DotnetStarter.Test/DotnetStarter.Test.csproj", "DotnetStarter.Test/"]
RUN dotnet restore "DotnetStarter.Test/DotnetStarter.Test.csproj"
COPY . .
WORKDIR "/src/DotnetStarter.Test"
RUN dotnet build "DotnetStarter.Test.csproj" -c Debug

# Install PowerShell
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN apt-get update
RUN apt-get install -y powershell

FROM build AS final
ENV SNAPSHOOTER_STRICT_MODE="true"
WORKDIR "/src/DotnetStarter.Test"
ENTRYPOINT ["dotnet", "test", "--no-build"]
