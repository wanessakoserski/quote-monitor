namespace QuoteMonitor.Domains
{
    public class TrackingQuote
    {
        public string Symbol { get; }
        private decimal SellingPrice { get; }
        private decimal PurchasePrice { get; }
        private decimal LastTrack { get; set; }

        public TrackingQuote(string symbol, decimal sellingPrice, decimal purchasePrice)
        {
            this.Symbol = symbol;
            this.SellingPrice = sellingPrice;
            this.PurchasePrice = purchasePrice;
        }

        public bool ShouldBuy(decimal currentPrice)
        {
            return currentPrice <= PurchasePrice;
        }

        public bool ShouldSell(decimal currentPrice)
        {
            return currentPrice >= SellingPrice;
        }

        public bool isNewPrice(decimal currentPrice)
        {
            if (currentPrice == LastTrack)
            {
                Console.WriteLine("Doesn't change quote price");
                return false;
            }

            LastTrack = currentPrice;

            return true;
        }
    }
}
