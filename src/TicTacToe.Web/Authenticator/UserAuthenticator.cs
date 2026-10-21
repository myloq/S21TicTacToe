using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Web.Authenticator;

public class UserAuthenticator : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Basic";

    private readonly IUserService _userService;

    public UserAuthenticator(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IUserService userService)
        : base(options, logger, encoder)
    {
        _userService = userService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(header))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (!header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        var base64 = header["Basic ".Length..]; 

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid base64 in Authorization header"));
        }

        var decoded = Encoding.UTF8.GetString(bytes);

        var separator = decoded.IndexOf(':');
        if (separator < 0)
            return Task.FromResult(AuthenticateResult.Fail("Invalid credentials format"));

        var login = decoded[..separator];
        var password = decoded[(separator + 1)..];

        var userId = _userService.Authenticate(new SignUpRequest {Login = login, Password = password});
        if (userId is null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid login or password"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
        };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
    
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers.WWWAuthenticate = $"{SchemeName} realm=\"TicTacToe\"";
        await base.HandleChallengeAsync(properties);
    }
}