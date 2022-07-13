using System.Security.Claims;
using CC.Authority.Implementation.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace CC.Authority.Api.Controllers;

[AllowAnonymous]
[Route("/connect")]
public class AuthorizationController : Controller
{
    private const string CompanyIdClaimName = "company_id";

    private readonly UserManager<User> _userManager;

    public AuthorizationController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
            
            if (request is null)
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        ClaimsPrincipal claimsPrincipal;

        if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            if (user is null)
                return Unauthorized();

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized();

            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            identity.AddClaim(OpenIddictConstants.Claims.Subject, user.UserId.ToString(),
                OpenIddictConstants.Destinations.AccessToken);
            identity.AddClaim(OpenIddictConstants.Claims.Username, user.PrimaryEmail,
                OpenIddictConstants.Destinations.AccessToken);
            identity.AddClaim(CompanyIdClaimName, user.CompanyId.ToString(),
                OpenIddictConstants.Destinations.AccessToken);

            claimsPrincipal = new ClaimsPrincipal(identity);
            claimsPrincipal.SetResources("api");

            var scopes = request.GetScopes();

            claimsPrincipal.SetScopes(scopes);
        }
        else
        {
            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}