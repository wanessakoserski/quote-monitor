using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuoteMonitor.Domains;
using QuoteMonitor.Email;
using QuoteMonitor.QuoteProviders;
using QuoteMonitor.QuoteProviders.BrapiDev;
using QuoteMonitor.Services;
using System.Globalization;

namespace QuoteMonitor
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            
            if (args.Length != 3 && args.Length != 1)
            {
                Console.WriteLine("Uso esperado >> QuoteMonitor.exe <ATIVO> <PRECO_PARA_VENDA> <PRECO_PARA_COMPRA>");
                return;
            }

            Env.Load();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("emails-base.json", optional: false)
                .AddJsonFile("quotes-base.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            services.Configure<SmtpSettings>(configuration.GetSection("smtp"));
            services.Configure<EmailsBase>(configuration);
            services.Configure<QuotesBase>(configuration);

            services.AddSingleton<IQuoteProvider, BrapiDevQuoteProvider>();
            services.AddSingleton<EmailMessage>();
            services.AddSingleton<EmailSender>();
            services.AddSingleton<QuoteMonitorService>();

            var trackingQuotes = new List<TrackingQuote>();

            if (args.Length == 3)
            {
                string symbol = args[0].ToUpper();

                if (!decimal.TryParse(args[1], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sellingPrice))
                {
                    Console.WriteLine(">> Preço de venda inválido.");
                    return;
                }

                if (!decimal.TryParse(args[2], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal purchasePrice))
                {
                    Console.WriteLine(">> Preço de compra inválido.");
                    return;
                }

                if (sellingPrice <= 0 || purchasePrice <= 0)
                {
                    Console.WriteLine(">> Os preços devem ser maiores que zero.");
                    return;
                }

                var trackingQuote = new TrackingQuote(symbol, sellingPrice, purchasePrice);

                trackingQuotes.Add(trackingQuote);
            } else
            {
                string input = args[0].ToUpper();
                if (input != "ALL")
                {
                    Console.WriteLine(">> Comando inválido");
                    Console.WriteLine("Uso esperado >> QuoteMonitor.exe ALL");
                    return;
                }

                var quotesBase = configuration.Get<QuotesBase>();

                foreach (var quote in quotesBase.Quotes)
                {
                    trackingQuotes.Add(new TrackingQuote(quote.Symbol, quote.Sell, quote.Buy));
                }
            }

            var serviceProvider = services.BuildServiceProvider();

            var quoteMonitorService = serviceProvider.GetRequiredService<QuoteMonitorService>();

            await quoteMonitorService.Start(trackingQuotes);
        }
    }
}
