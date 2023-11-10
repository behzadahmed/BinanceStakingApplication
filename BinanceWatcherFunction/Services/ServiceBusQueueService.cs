using BinanceWatcherFunction.Interfaces;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceWatcherFunction.Services
{
    public class ServiceBusQueueService : IQueueService
    {
        private QueueClient _queueClient;

        public ServiceBusQueueService(string serviceBusConnectionString, string queueName)
        {
            _queueClient = new QueueClient(serviceBusConnectionString, queueName);
        }

        public async Task SendToQueue(string message)
        {
            var messageBody = new Message(Encoding.UTF8.GetBytes(message));
            await _queueClient.SendAsync(messageBody);
        }
    }
}
