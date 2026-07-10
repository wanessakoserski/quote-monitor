namespace QuoteMonitor
{
    public class QuotesBase
    {
        public List<Quote> Quotes { get; set; } = new List<Quote>();

        public class Quote
        {
            public string Symbol { get; set; } = string.Empty;
            public decimal Buy { get; set; }
            public decimal Sell { get; set; }
        }
    }
}
