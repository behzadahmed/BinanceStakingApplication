using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Net.Http;
using System.Net.Http.Json;
using System.Linq;
using BinanceWatcherFunction.Models;
using BinanceWatcherFunction.Interfaces;
using BinanceWatcherFunction.Configuration;
using Microsoft.Extensions.Options;

namespace BinanceWatcherFunction
{
    public class BinanceWatcherFunction
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BinanceWatcherFunction> _logger;  
        private readonly string _url;
        private readonly IDataHandlerService _handlerService;


        public BinanceWatcherFunction(IMongoDbService mongoDbService, HttpClient httpClient, ILogger<BinanceWatcherFunction> logger, IOptions<BinanceApiConfiguration> config, IDataHandlerService handlerService)
        {
            _mongoDbService = mongoDbService;
            _httpClient = httpClient;
            _logger = logger;   
            _url = config.Value.ApiUri;
            _handlerService=handlerService;

        }

        [FunctionName("BinanceWatcherFunction")]
        public async Task Run([TimerTrigger("%TimerSchedule%")] TimerInfo myTimer)
        {
            try
            {
                _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            

                var httpResponseMessage = await _httpClient.GetAsync(_url);
                var result = await httpResponseMessage.Content.ReadFromJsonAsync<Result>();

                FilterDefinitionBuilder<ProjectData> dataFilterBuilder = Builders<ProjectData>.Filter;
                FilterDefinitionBuilder<CryptoList> cryptoListFilterBuilder = Builders<CryptoList>.Filter;

                foreach (var incomingData in result.Data)
                {
                    var dataFilter = dataFilterBuilder.Eq(existingItem => existingItem.Asset, incomingData.Asset);
                    var existingData = await _mongoDbService.FindDataAsync(dataFilter);
                    var cryptoFilter = cryptoListFilterBuilder.Eq(e => e.Crypto, incomingData.Asset);

                    if (existingData != null)
                    {
                        await _handlerService.HandleExistingData(incomingData, existingData, dataFilter, cryptoFilter);
                    }
                    else
                    {
                        await _handlerService.HandleNewData(incomingData);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error");
            }
        }
    }
}

