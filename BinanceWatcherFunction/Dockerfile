FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BinanceWatcherFunction.csproj", "."]

ENV AzureWebJobsStorage="DefaultEndpointsProtocol=https;AccountName=storageaccounttest8274;AccountKey=Fv2G9hB4Zc/0JHdumEZIawdiEOwCTQ2NSKfoQl7B6SmIkUfj85AUVN69fFDESoN6nNdqsP/5YZ3eNDI7keCzbg==;EndpointSuffix=core.windows.net"
ENV FUNCTIONS_WORKER_RUNTIME="dotnet"
ENV MongoDbServiceConfiguration__MongoClientUri="mongodb://localhost:27017"
ENV TelegramBotClientConfiguration__ApiKey="2126359628:AAG9CBk-I8OGDxwVukkdyH--aEFQ6wJJGto"
ENV TelegramBotClientConfiguration__ChannelId="-1001405726698"
ENV BinanceApiConfiguration__ApiUri="https://www.binance.com/gateway-api/v1/friendly/pos/union?status=ALL&pageSize=100"


RUN dotnet restore "./BinanceWatcherFunction.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BinanceWatcherFunction.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BinanceWatcherFunction.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true