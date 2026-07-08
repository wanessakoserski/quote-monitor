using QuoteMonitor.Domains;
using QuoteMonitor.Email;
using QuoteMonitor.QuoteProviders;

namespace QuoteMonitor.Services
{
    internal class QuoteMonitorService
    {
        private readonly IQuoteProvider _quoteProvider;
        private readonly EmailMessage _emailMessage;
        private readonly EmailSender _emailSender;

        public QuoteMonitorService(
            IQuoteProvider quoteProvider, 
            EmailMessage emailMessage, 
            EmailSender emailSender)
        {
            _quoteProvider = quoteProvider;
            _emailMessage = emailMessage;
            _emailSender = emailSender;
        }

        public async Task Start(TrackingQuote trackingQuote, int watchDelay = 90)
        {
            while (true)
            {
                CurrentQuote? currentQuote = await _quoteProvider.GetCurrentQuoteAsync(trackingQuote.Symbol);

                if (currentQuote is null || !trackingQuote.isNewPrice(currentQuote.Price))
                {
                    await Task.Delay(TimeSpan.FromSeconds(watchDelay * 0.6));
                    continue;
                }

                if (trackingQuote.ShouldSell(currentQuote.Price))
                {
                    Console.WriteLine("Should sell");

                    var message = _emailMessage.CreateSellAdviceMessage(trackingQuote.Symbol, currentQuote.Price);
                    await _emailSender.SendAsync(message);
                }
                else if (trackingQuote.ShouldBuy(currentQuote.Price))
                {
                    Console.WriteLine("Should buy");

                    var message = _emailMessage.CreateBuyAdviceMessage(trackingQuote.Symbol, currentQuote.Price);
                    await _emailSender.SendAsync(message);
                }

                await Task.Delay(TimeSpan.FromSeconds(watchDelay));
            }
        }
    }
}
