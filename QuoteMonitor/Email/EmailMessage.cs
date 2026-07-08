namespace QuoteMonitor.Email
{
    internal class EmailMessage
    {
        public Message CreateBuyAdviceMessage(string symbol, decimal currentPrice)
        {
            return new Message
            {
                Subject = $"Recomendação de compra: {symbol}",
                Body = $"A cotação de {symbol} atingiu R$ {currentPrice:F2}. Pode ser um bom momento para comprar."
            };
        }

        public Message CreateSellAdviceMessage(string symbol, decimal currentPrice)
        {
            return new Message
            {
                Subject = $"Recomendação de venda: {symbol}",
                Body = $"A cotação de {symbol} atingiu R$ {currentPrice:F2}. Pode ser um bom momento para vender."
            };
        }
    }
}
