FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["PrivateCA.API/PrivateCA.API.csproj", "PrivateCA.API/"]
COPY ["PrivateCA.Core/PrivateCA.Core.csproj", "PrivateCA.Core/"]
RUN dotnet restore "PrivateCA.API/PrivateCA.API.csproj"
COPY . .
WORKDIR "/src/PrivateCA.API"
RUN dotnet build "PrivateCA.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrivateCA.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrivateCA.API.dll"]
