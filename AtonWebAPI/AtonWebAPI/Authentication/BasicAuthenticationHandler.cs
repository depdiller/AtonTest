using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using AtonWebAPI.DataTransferObjects;
using AtonWebAPI.Models;
using AtonWebAPI.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AtonWebAPI.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IRepository _userService;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IRepository userService)
        : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // skip authentication if endpoint has [AllowAnonymous] attribute
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            return Task.FromResult(AuthenticateResult.NoResult());

        if (!Request.Headers.ContainsKey("Authorization"))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

        UserWithoutTechInfoDto userWithoutTechInfo = null;
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] {':'}, 2);
            var username = credentials[0];
            var password = credentials[1];
            userWithoutTechInfo = _userService.Authenticate(username, password);
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }

        if (userWithoutTechInfo == null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userWithoutTechInfo.Login),
            new Claim(ClaimTypes.Role, userWithoutTechInfo.Admin ? Role.Admin : Role.OrdinaryUser),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}