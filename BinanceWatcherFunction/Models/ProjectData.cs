using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace BinanceWatcherFunction.Models
{
    [BsonIgnoreExtraElements]
    public class ProjectData
    {
        public string Asset { get; set; }
        public decimal AnnualInterestRate { get; set; }
        public decimal MinPurchaseAmount { get; set; }
        public int RedeemPeriod { get; set; }
        public List<Project>? Projects { get; set; }
    }
}
