using System.Text.Json;
using QuoteMonitor.Domains;

namespace QuoteMonitor.QuoteProviders.BrapiDev
{
    internal class BrapiDevQuoteProvider : IQuoteProvider
    {
        private const string BaseUrl = "https://brapi.dev/api/v2/stocks";
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<CurrentQuote?> GetCurrentQuoteAsync(string symbol)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/quote?symbols={symbol}");

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var brapiResponse = JsonSerializer.Deserialize<BrapiDevQuoteResponse>(json)
                    ?? throw new InvalidOperationException("Formato inválido de resposta");

                if (brapiResponse.Results.Count == 0)
                {
                    throw new InvalidOperationException("Nenhuma cotação encontrada");
                }

                var quote = brapiResponse.Results.First();

                return new CurrentQuote(quote.Symbol, quote.Metadata.RegularMarketPrice, brapiResponse.RequestedAt);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Não foi resgatar informações da cotação >> " + ex.ToString());

                return null;
            }
        }
    }
}
