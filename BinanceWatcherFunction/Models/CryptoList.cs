using MongoDB.Bson.Serialization.Attributes;

namespace BinanceWatcherFunction.Models
{
    [BsonIgnoreExtraElements]
    public class CryptoList
    {
        public string Crypto { get; set; }

    }
}
