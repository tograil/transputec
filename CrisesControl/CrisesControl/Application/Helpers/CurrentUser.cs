using OpenIddict.Abstractions;

namespace CrisesControl.Api.Application.Helpers;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId => int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == OpenIddictConstants.Claims.Subject)?.Value ?? "0");

    public string UserName =>
        _httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == OpenIddictConstants.Claims.Username)?.Value ??
        string.Empty;
    public int CompanyId => int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == "company_id")?.Value ?? "0");
}