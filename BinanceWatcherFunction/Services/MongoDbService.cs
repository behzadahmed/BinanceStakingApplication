using BinanceWatcherFunction.Configuration;
using BinanceWatcherFunction.Interfaces;
using BinanceWatcherFunction.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceWatcherFunction.Services
{


    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbService> _logger;

        public MongoDbService(IOptions<MongoDbServiceConfiguration> config, ILogger<MongoDbService> logger)
        {
            var mongoClientUri = config.Value.MongoClientUri;
            if (string.IsNullOrEmpty(mongoClientUri))
            {
                throw new ArgumentException("MongoClientUri configuration is missing");
            }

            var client = new MongoClient(mongoClientUri);
            _database = client.GetDatabase("Crypto");
            _logger = logger;
        }

        public IMongoCollection<ProjectData> GetStakingCollection()
        {
            return _database.GetCollection<ProjectData>("StakingOptions");
        }

        public IMongoCollection<CryptoList> GetCryptoCollection()
        {
            return _database.GetCollection<CryptoList>("CryptoList");
        }

        public async Task<ProjectData> FindDataAsync(FilterDefinition<ProjectData> dataFilter)
        {
            try
            {
                var collection = GetStakingCollection();
                return await collection.Find(dataFilter).SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding data");
                throw;
            }
        }

        public async Task ReplaceDataAsync(FilterDefinition<ProjectData> dataFilter, ProjectData data)
        {
            try
            {
                var collection = GetStakingCollection();
                await collection.ReplaceOneAsync(dataFilter, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error replacing data");
                throw;
            }
        }

        public async Task InsertDataAsync(ProjectData data)
        {
            try
            {
                var collection = GetStakingCollection();
                await collection.InsertOneAsync(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting data");
                throw;
            }
        }

        public async Task<bool> CryptoExists(FilterDefinition<CryptoList> cryptoFilter)
        {
            try
            {
                var collection = GetCryptoCollection();
                return await collection.Find(cryptoFilter).AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if crypto exists");
                throw;
            }
        }
    }



}
