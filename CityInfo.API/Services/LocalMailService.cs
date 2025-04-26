namespace CityInfo.API.Services
{
    public class LocalMailService
    {
        public string _mailTo = "abc@gmail.com";
        public string _mailFrom = "xyz@gmail.com";

        public void Send(string subject, string message)
        {
            //Send mail - output to debug window
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with LocalMailService.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
