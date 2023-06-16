using Microsoft.AspNetCore.Http;

namespace CC.Authority.Implementation.Helpers
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId => int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == "sub")?.Value ?? "0");

        public string UserName =>
            _httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == "username")?.Value ??
            string.Empty;
        public int CompanyId => int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == "company_id")?.Value ?? "0");
        public string TimeZone => _httpContextAccessor.HttpContext?.User.FindFirst(x => x.Type == "time_zone")?.Value ??
                                  "GMT Standard Time";
    }
}