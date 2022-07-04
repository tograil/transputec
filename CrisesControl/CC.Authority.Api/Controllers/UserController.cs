using System.Security.Claims;
using CC.Authority.Implementation.Data;
using CC.Authority.SCIM.Protocol;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;

namespace CC.Authority.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly CrisesControlAuthContext _authContext;

        private const string CompanyIdClaimName = "company_id";

        public UserController(CrisesControlAuthContext authContext)
        {
            _authContext = authContext;
        }

        [HttpGet("superadmins")]
        public IActionResult GetSuperAdmins()
        {
            return Ok(_authContext.Users.Where(x => x.UserRole == "SUPERADMIN").Select(x => new { Id = x.UserId, Login = x.PrimaryEmail, Name = string.Join(" ", x.FirstName, x.LastName) }).ToArray());
        }

        [HttpPost("token")]
        public IActionResult GenerateToken(int id)
        {
            var user = _authContext.Users.First(x => x.UserId == id);

            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            identity.AddClaim(OpenIddictConstants.Claims.Subject, user.UserId.ToString(),
                OpenIddictConstants.Destinations.AccessToken);
            identity.AddClaim(OpenIddictConstants.Claims.Username, user.PrimaryEmail,
                OpenIddictConstants.Destinations.AccessToken);
            identity.AddClaim(CompanyIdClaimName, user.CompanyId.ToString(),
                OpenIddictConstants.Destinations.AccessToken);

            var claimsPrincipal = new ClaimsPrincipal(identity);
            claimsPrincipal.SetResources("api");

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.Now.AddYears(2),
                IsPersistent = true
            };

            return SignIn(claimsPrincipal, authProperties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}