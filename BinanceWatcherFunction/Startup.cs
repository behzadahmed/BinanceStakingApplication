using BinanceWatcherFunction.Interfaces;
using BinanceWatcherFunction.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.Generic;
using System;
using System.Net.Http;
using Telegram.Bot;
using BinanceWatcherFunction.Configuration;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(BinanceWatcherFunction.Startup))]

namespace BinanceWatcherFunction
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => level >= LogLevel.Information);
            });

            builder.Services.AddScoped<IDataHandlerService, DataHandlerService>();
            builder.Services.AddScoped<IQueueService, ServiceBusQueueService>();

            var configuration = builder.GetContext().Configuration;

            builder.Services.Configure<MongoDbServiceConfiguration>(configuration.GetSection(nameof(MongoDbServiceConfiguration)));
            builder.Services.AddSingleton<IMongoDbService, MongoDbService>();
   
            builder.Services.Configure<BinanceApiConfiguration>(configuration.GetSection(nameof(BinanceApiConfiguration)));

        }

    }
}
