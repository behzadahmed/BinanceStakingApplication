using BinanceWatcherFunction.Models;
using BinanceWatcherFunction.Services;
using MongoDB.Driver;

using System.Threading.Tasks;

namespace BinanceWatcherFunction.Interfaces
{
    public interface IDataHandlerService
    {
        Task HandleExistingData( ProjectData incomingData, ProjectData existingData, FilterDefinition<ProjectData> dataFilter, FilterDefinition<CryptoList> cryptoFilter);
        Task HandleNewData(ProjectData incomingData);  

    }

}

