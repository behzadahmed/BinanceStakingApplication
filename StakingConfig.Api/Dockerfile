FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StakingConfig.Api/StakingConfig.Api.csproj", "StakingConfig.Api/"]
RUN dotnet restore "StakingConfig.Api/StakingConfig.Api.csproj"
COPY . .
WORKDIR "/src/StakingConfig.Api"
RUN dotnet build "StakingConfig.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StakingConfig.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StakingConfig.Api.dll"]