namespace QuoteMonitor.Email
{
    internal class SmtpSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string Name { get; set; } = "";
    }
}
