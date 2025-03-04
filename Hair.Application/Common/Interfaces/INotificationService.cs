namespace Hair.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendSmsAsync(string toPhoneNumber, string messageText);
}