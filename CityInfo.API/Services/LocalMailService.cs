namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        public LocalMailService(IConfiguration configuration)
        {
            this._mailTo = configuration["mailSettings:mailToAddress"];
            this._mailFrom = configuration["mailSettings:mailFromAddress"];
        }

        private readonly string _mailTo = string.Empty;
        private readonly string _mailFrom = string.Empty;

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
