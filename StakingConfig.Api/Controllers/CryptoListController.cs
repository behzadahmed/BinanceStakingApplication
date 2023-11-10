using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using StakingConfig.Api.Models;
using System.Text.Json;

namespace StakingConfig.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoListController : ControllerBase
    {
        private readonly IMongoCollection<CryptoList> _cryptoList;
        private readonly ILogger<CryptoListController> _logger;

        public CryptoListController(ILogger<CryptoListController> logger, IMongoClient client)
        {
            _logger = logger;
            var database = client.GetDatabase("Crypto");
            _cryptoList = database.GetCollection<CryptoList>("CryptoList");
        }

        [HttpGet]
        public async Task<IEnumerable<CryptoList>> Get()
        {
            _logger.LogInformation("Getting all items from CryptoList");
            return await _cryptoList.Find(new BsonDocument()).ToListAsync();
        }

        [HttpPost]
        public async Task Post([FromBody] CryptoList item)
        {
            var existingItem = await _cryptoList.Find(c => c.Crypto == item.Crypto).FirstOrDefaultAsync();

            if (existingItem != null)
            {
                _logger.LogError($"Item with Crypto: {item.Crypto} already exists.");
                return;
            }

            _logger.LogInformation($"Adding item to CryptoList: {item.Crypto}");
            await _cryptoList.InsertOneAsync(item);
        }


        [HttpDelete("{id}")]
        public async Task Delete(string crypto)
        {
            _logger.LogInformation($"Removing item from CryptoList with name: {crypto}");
            await _cryptoList.DeleteOneAsync(c => c.Crypto == crypto);
        }
    }
}