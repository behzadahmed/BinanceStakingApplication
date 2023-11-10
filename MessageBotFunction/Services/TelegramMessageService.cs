using MessageBotFunction.Configuration;
using MessageBotFunction.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

namespace MessageBotFunction.Services
{
    public class TelegramMessageService : IMessageService
    {
        private readonly TelegramBotClient _bot;
        private readonly string _channelId;
        private readonly ILogger<TelegramMessageService> _logger;


        public TelegramMessageService(IOptions<TelegramBotClientConfiguration> config, ILogger<TelegramMessageService> logger)
        {
            _bot = new TelegramBotClient(config.Value.ApiKey);
            _channelId = config.Value.ChannelId;
            _logger = logger;
        }

        public async Task SendMessage(string message)
        {
            int maxRetryAttempts = 3;
            int retryAttempt = 0;

            while (retryAttempt < maxRetryAttempts)
            {
                try
                {
                    await _bot.SendTextMessageAsync(_channelId, message);
                    await Task.Delay(1000);
                    _logger.LogInformation("Message sent successfully");
                    break;
                }
                catch (ApiRequestException ex) when (ex.ErrorCode == 429)
                {
                    _logger.LogWarning($"Rate limit exceeded: {ex.Message}. Retrying in 1 second...");
                    retryAttempt++;
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred while sending the message: {ex.Message}");
                    break;
                }
            }
        }

        //telegram limit of 20 messages a minuite
        public async Task<int> SendMessageWithRateLimiting(string message, int count)
        {
            if (!string.IsNullOrEmpty(message))
            {
                await SendMessage(message);
                count++;
                if (count == 20)
                {
                    await Task.Delay(41000);
                    count = 0;
                }
            }
            return count;
        }

       
    }
}


