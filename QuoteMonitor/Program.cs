using QuoteMonitor.Domains;
using System.Globalization;

namespace QuoteMonitor
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Uso esperado >> QuoteMonitor.exe <ATIVO> <PRECO_PARA_VENDA> <PRECO_PARA_COMPRA>");
                return;
            }

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
        }
    }
}
