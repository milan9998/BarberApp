using Hair.Application.Common.Interfaces;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;

namespace Hair.Infrastructure.Services;

public class VonageNotificationService : INotificationService
{
    private readonly string _apiKey ;
    private readonly string _apiSecret;

    public VonageNotificationService()
    {
        _apiKey = "11831689";
        _apiSecret = "0XVE9gvGjj4exLP4";
    }
    
    
    public async Task SendSmsAsync(string toPhoneNumber, string messageText)
    {
        var credentials = Credentials.FromApiKeyAndSecret(_apiKey, _apiSecret);
        var vonageClient = new VonageClient(credentials);

        var response = await Task.Run(() => vonageClient.SmsClient.SendAnSmsAsync(new SendSmsRequest
        {
            To = toPhoneNumber,
            From = "BarberApp", // Sender ID
            Text = messageText
        }));

        if (response.Messages[0].Status != "0")
        {
            throw new Exception($"Error sending SMS: {response.Messages[0].ErrorText}");
        }
    }
}