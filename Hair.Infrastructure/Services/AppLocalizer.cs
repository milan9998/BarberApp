using Hair.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Hair.Infrastructure.Services;

public class AppLocalizer(IHttpContextAccessor httpContextAccessor) : IAppLocalizer
{
    public bool IsSerbian
    {
        get
        {
            var header = httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();
            if (string.IsNullOrWhiteSpace(header))
            {
                return false;
            }

            var primary = header.Split(',')[0].Trim().ToLowerInvariant();
            return primary.StartsWith("sr");
        }
    }

    public string T(string english, string serbian) => IsSerbian ? serbian : english;
}
