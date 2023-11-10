using BinanceWatcherFunction.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceWatcherFunction.Interfaces
{
    public interface IQueueService
    {
        Task SendToQueue(string message);

    }

}

