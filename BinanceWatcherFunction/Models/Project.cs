namespace BinanceWatcherFunction.Models
{
    public class Project
    {
        public int Id { get; set; }
        public bool SellOut { get; set; }
        public int Duration { get; set; }
        public Config Config { get; set; }

    }
}
