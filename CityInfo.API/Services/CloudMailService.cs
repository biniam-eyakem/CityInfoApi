namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        public CloudMailService(IConfiguration configuration)
        {
            this._mailTo = configuration["mailSettings:mailToAddress"];
            this._mailFrom = configuration["mailSettings.mailFromAddress"];
        }

        private readonly string _mailTo = string.Empty;
        private readonly string _mailFrom = string.Empty;

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
