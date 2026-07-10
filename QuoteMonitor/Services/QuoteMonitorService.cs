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

        public async Task Start(List<TrackingQuote> trackingQuotes, int watchDelay = 30)
        {
            while (true)
            {
                foreach (var trackingQuote in trackingQuotes)
                {
                    CurrentQuote? currentQuote = await _quoteProvider.GetCurrentQuoteAsync(trackingQuote.Symbol);

                    if (currentQuote is null)
                    {
                        continue;
                    }

                    if (!trackingQuote.IsNewPrice(currentQuote.Price))
                    {
                        continue;
                    }

                    if (trackingQuote.ShouldSell(currentQuote.Price))
                    {
                        Console.WriteLine("Should sell");

                        try
                        {
                            var message = _emailMessage.CreateSellAdviceMessage(trackingQuote.Symbol, currentQuote.Price);
                            await _emailSender.SendAsync(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Problemas com a conexão com o host - envio de email sobre venda >> " + ex.ToString());
                        }
                    }
                    else if (trackingQuote.ShouldBuy(currentQuote.Price))
                    {
                        Console.WriteLine("Should buy");

                        try
                        {
                            var message = _emailMessage.CreateBuyAdviceMessage(trackingQuote.Symbol, currentQuote.Price);
                            await _emailSender.SendAsync(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Problemas com a conexão com o host - envio de email sobre compra >> " + ex.ToString());
                        }
                    }

                }
                
                await Task.Delay(TimeSpan.FromSeconds(watchDelay));
            }
        }
    }
}
