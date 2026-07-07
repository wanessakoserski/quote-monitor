using QuoteMonitor.Domains;

namespace QuoteMonitor.QuoteProviders
{
    internal interface IQuoteProvider
    {
        Task<CurrentQuote?> GetCurrentQuoteAsync(string symbol);
    }
}
