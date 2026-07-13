namespace Hair.Application.Common.Configuration;

public class EmailSettings
{
    public const string SectionName = "Email";

    public string Host { get; set; } = "smtp.zoho.com";
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Barber Control HQ";
}

public class AppUrlSettings
{
    public const string SectionName = "AppUrls";

    public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
    public string ApiBaseUrl { get; set; } = "http://localhost:5045";
}
