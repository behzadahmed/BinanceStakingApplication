using System.Collections.Generic;

namespace BinanceWatcherFunction.Models
{
    public class Result
    {
        public List<ProjectData> Data { get; set; }
        public int? Code { get; set; }
        public string Message { get; set; }
        public string MessageDetail { get; set; }

    }
}
