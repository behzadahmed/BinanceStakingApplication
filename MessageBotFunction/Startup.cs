using MessageBotFunction.Interfaces;
using MessageBotFunction.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Collections.Generic;
using System;
using System.Net.Http;
using Telegram.Bot;
using MessageBotFunction.Configuration;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(MessageBotFunction.Startup))]

namespace MessageBotFunction
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
 
            builder.Services.AddScoped<IMessageService, TelegramMessageService>();

            var configuration = builder.GetContext().Configuration;

            builder.Services.Configure<TelegramBotClientConfiguration>(configuration.GetSection(nameof(TelegramBotClientConfiguration)));
            builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>();


        }

    }
}
