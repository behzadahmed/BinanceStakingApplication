using BinanceWatcherFunction.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceWatcherFunction.Interfaces
{
    public interface IMongoDbService
    {
        IMongoCollection<ProjectData> GetStakingCollection();
        IMongoCollection<CryptoList> GetCryptoCollection();
        Task<ProjectData> FindDataAsync(FilterDefinition<ProjectData> dataFilter);
        Task ReplaceDataAsync(FilterDefinition<ProjectData> dataFilter, ProjectData data);
        Task InsertDataAsync(ProjectData data);
        Task<bool> CryptoExists(FilterDefinition<CryptoList> cryptoFilter);

    }
}
