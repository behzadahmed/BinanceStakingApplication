using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBotFunction.Interfaces
{
    public interface IMessageService
    {
        Task SendMessage(string message);
        Task<int> SendMessageWithRateLimiting(string message, int count);

    }

}

