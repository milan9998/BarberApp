using Microsoft.AspNetCore.Http;

namespace Hair.Application.Common.Interfaces;

public interface IAppLocalizer
{
    bool IsSerbian { get; }

    string T(string english, string serbian);
}
