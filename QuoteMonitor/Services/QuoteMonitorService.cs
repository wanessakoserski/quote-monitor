using QuoteMonitor.Domains;
using QuoteMonitor.QuoteProviders;

namespace QuoteMonitor.Services
{
    internal class QuoteMonitorService
    {
        private readonly IQuoteProvider _quoteProvider;

        public QuoteMonitorService(IQuoteProvider quoteProvider)
        {
            _quoteProvider = quoteProvider;
        }

        public async Task Start(TrackingQuote trackingQuote, int watchDelay = 90)
        {
            while (true)
            {
                CurrentQuote? currentQuote = await _quoteProvider.GetCurrentQuoteAsync(trackingQuote.Symbol);

                if (currentQuote is null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(watchDelay * 0.5));

                    continue;
                }

                if (trackingQuote.ShouldSell(currentQuote.Price))
                {
                    Console.WriteLine("Should sell");
                }
                else if (trackingQuote.ShouldBuy(currentQuote.Price))
                {
                    Console.WriteLine("Should buy");
                }

                await Task.Delay(TimeSpan.FromSeconds(watchDelay));
            }
        }
    }
}
