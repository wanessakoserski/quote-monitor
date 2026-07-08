namespace QuoteMonitor.Domains
{
    public class CurrentQuote
    {
        public string Symbol { get; }
        public decimal Price { get; }
        public DateTime RequestAt { get; }

        public CurrentQuote(string symbol, decimal price, DateTime requestAt)
        {
            this.Symbol = symbol;
            this.Price = price;
            this.RequestAt = requestAt;
        }
    }
}
