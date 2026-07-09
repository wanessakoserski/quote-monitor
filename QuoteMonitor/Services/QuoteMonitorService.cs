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

        public async Task Start(TrackingQuote trackingQuote, int watchDelay = 60)
        {
            while (true)
            {
                CurrentQuote? currentQuote = await _quoteProvider.GetCurrentQuoteAsync(trackingQuote.Symbol);

                if (currentQuote is null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(watchDelay * 0.1));
                    continue;
                }

                if (!trackingQuote.isNewPrice(currentQuote.Price))
                {
                    await Task.Delay(TimeSpan.FromSeconds(watchDelay * 0.6));
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

                await Task.Delay(TimeSpan.FromSeconds(watchDelay));
            }
        }
    }
}
