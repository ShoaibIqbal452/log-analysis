FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app

COPY NuGet.config .

COPY src/LogAnalysis.Api/*.csproj src/LogAnalysis.Api/
COPY src/LogAnalysis.Core/*.csproj src/LogAnalysis.Core/

RUN dotnet restore src/LogAnalysis.Api/LogAnalysis.Api.csproj

COPY . .
RUN dotnet publish src/LogAnalysis.Api -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=0 /app/out .
ENTRYPOINT ["dotnet", "LogAnalysis.Api.dll"]
