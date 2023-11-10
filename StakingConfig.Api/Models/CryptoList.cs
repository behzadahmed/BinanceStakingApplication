using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StakingConfig.Api.Models
{
    public class CryptoList
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Crypto { get; set; }
    }
}
