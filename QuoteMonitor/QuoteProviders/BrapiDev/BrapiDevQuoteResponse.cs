using System.Text.Json.Serialization;

namespace QuoteMonitor.QuoteProviders.BrapiDev
{
    public class BrapiDevQuoteResponse
    {
        [JsonPropertyName("results")]
        public List<Result> Results { get; set; } = new List<Result>();

        [JsonPropertyName("requestedAt")]
        public DateTime RequestedAt { get; set; }

        public class Result
        {
            [JsonPropertyName("symbol")]
            public string Symbol { get; set; } = string.Empty;

            [JsonPropertyName("data")]
            public Data Metadata { get; set; } = new Data();

            public class Data
            {
                [JsonPropertyName("longName")]
                public string LongName { get; set; } = string.Empty;

                [JsonPropertyName("regularMarketPrice")]
                public decimal RegularMarketPrice { get; set; }
            }
        }
    }
}
