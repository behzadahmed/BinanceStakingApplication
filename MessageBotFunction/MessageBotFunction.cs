using System;
using System.Threading.Tasks;
using MessageBotFunction.Interfaces;
using MessageBotFunction.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MessageBotFunction
{
    public class MessageBotFunction
    {
        private readonly IMessageService _messageService;

        public MessageBotFunction(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [FunctionName("MessageBotFunction")]
        public async Task Run([ServiceBusTrigger("%myqueue%", Connection = "ServiceBusConnectionString")] string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            await _messageService.SendMessage(myQueueItem);
        }
    }
}