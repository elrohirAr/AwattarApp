using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwattarApp
{
    public class MarketData
    {
        [JsonProperty("start_timestamp")]
        public long StartTimestamp { get; set; }

        [JsonProperty("end_timestamp")]
        public long EndTimestamp { get; set; }
        public double MarketPrice { get; set; }
        public string Unit { get; set; } = "";
    }

    public class MarketDataResponse
    {
        public string Object { get; set; } = "";
        public List<MarketData> Data { get; set; } = new();
        public string Url { get; set; } = "";
    }

}
