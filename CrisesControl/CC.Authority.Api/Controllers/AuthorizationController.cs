using System.Security.Claims;
using CC.Authority.Implementation.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace CC.Authority.Api.Controllers;

[AllowAnonymous]
[Route("/connect")]
public class AuthorizationController : Controller {
    private const string CompanyIdClaimName = "company_id";

    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthorizationController(UserManager<User> userManager, SignInManager<User> signInManager) {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("token")]
    [Produces("application/json")]
    public async Task<IActionResult> Exchange() {
        var oidcRequest = HttpContext.GetOpenIddictServerRequest();


        if (oidcRequest is null)
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (oidcRequest.IsPasswordGrantType())
            return await TokensForPasswordGrantType(oidcRequest);

        if (oidcRequest.IsRefreshTokenGrantType()) {
            return await RefreshTokensForGrantType(oidcRequest);
        }

        //if (oidcRequest.GrantType == "custom_flow_name") {
        //    // return tokens for custom flow
        //}

        return BadRequest(new OpenIddictResponse {
            Error = OpenIddictConstants.Errors.UnsupportedGrantType
        });
    }


    private async Task<IActionResult> TokensForPasswordGrantType(OpenIddictRequest request) {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return Unauthorized();

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized();


        var identity = new ClaimsIdentity(
            TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.UserId.ToString(), OpenIddictConstants.Destinations.AccessToken);
        identity.AddClaim(OpenIddictConstants.Claims.Username, user.PrimaryEmail, OpenIddictConstants.Destinations.AccessToken);
        identity.AddClaim(CompanyIdClaimName, user.CompanyId.ToString(), OpenIddictConstants.Destinations.AccessToken);

        var claimsPrincipal = new ClaimsPrincipal(identity);
        //claimsPrincipal.SetResources("api");
        var scopes = request.GetScopes().Add(OpenIddictConstants.Scopes.OfflineAccess);
        //scopes = scopes.Add(OpenIddictConstants.Scopes.OfflineAccess);
        claimsPrincipal.SetScopes(scopes);

        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<IActionResult> RefreshTokensForGrantType(OpenIddictRequest request) {

        // Retrieve the claims principal stored in the refresh token.
        var info = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        // Retrieve the user profile and validate the
        // security stamp stored in the refresh token.
        var user = await _signInManager.ValidateSecurityStampAsync(info.Principal);
        if (user == null) {
            return BadRequest(new OpenIddictResponse {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "The refresh token is no longer valid."
            });
        }

        // Ensure the user is still allowed to sign in.
        if (!await _signInManager.CanSignInAsync(user)) {
            return BadRequest(new OpenIddictResponse {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "The user is no longer allowed to sign in."
            });
        }

        // Create a new authentication ticket, but reuse the properties stored
        // in the refresh token, including the scopes originally granted.
        var ticket = await CreateTicketAsync(request, user, info.Properties);

        return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);

    }

    private async Task<AuthenticationTicket> CreateTicketAsync(OpenIddictRequest request, User user, AuthenticationProperties properties = null) {
        // Create a new ClaimsPrincipal containing the claims that will be used to create an id_token, a token or a code.
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        // Create a new authentication ticket holding the user identity.
        var ticket = new AuthenticationTicket(principal, properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them to a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        var identity = new ClaimsIdentity(
           TokenValidationParameters.DefaultAuthenticationType,
           OpenIddictConstants.Claims.Name,
           OpenIddictConstants.Claims.Role);

        foreach (var claim in ticket.Principal.Claims) {
            // Never include the security stamp in the access and identity tokens, as it's a secret value.

            var destinations = new List<string>
            {
                OpenIddictConstants.Destinations.AccessToken
            };
            // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
            // The other claims will only be added to the access_token, which is encrypted when using the default format.
            if ((claim.Type == OpenIddictConstants.Claims.Name && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Profile)) ||
                (claim.Type == OpenIddictConstants.Claims.Email && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Email)) ||
                (claim.Type == OpenIddictConstants.Claims.Role && ticket.Principal.HasScope(OpenIddictConstants.Scopes.Roles))) {
                destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
            }
            claim.SetDestinations(destinations);
        }
        return ticket;
    }

}