namespace QuoteMonitor.Domains
{
    public class TrackingQuote
    {
        public string Symbol { get; }
        private decimal SellingPrice { get; }
        private decimal PurchasePrice { get; }

        public TrackingQuote(string symbol, decimal sellingPrice, decimal purchasePrice)
        {
            Symbol = symbol;
            SellingPrice = sellingPrice;
            PurchasePrice = purchasePrice;
        }

        public bool ShouldBuy(decimal currentPrice)
        {
            return currentPrice <= PurchasePrice;
        }

        public bool ShouldSell(decimal currentPrice)
        {
            return currentPrice >= SellingPrice;
        }
    }
}
