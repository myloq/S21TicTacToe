using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TicTacToe.Domain.Interface;

namespace TicTacToe.Web.Controller;

public class UserController : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Basic";

    private readonly IUserService _userService;

    public UserController(
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
        // ─── 1. Читаем заголовок Authorization ────────────────────────────
        var header = Request.Headers.Authorization.ToString();

        // Если заголовка нет — это не наша проблема, пусть решает кто-то другой
        if (string.IsNullOrEmpty(header))
            return Task.FromResult(AuthenticateResult.NoResult());

        // Если это не Basic — тоже не наша схема
        if (!header.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(AuthenticateResult.NoResult());

        // ─── 2. Декодируем base64 ─────────────────────────────────────────
        var base64 = header["Basic ".Length..]; // всё после "Basic "

        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(base64);
        }
        catch (FormatException)
        {
            // Клиент прислал мусор вместо base64 — это 401, а не 500
            return Task.FromResult(AuthenticateResult.Fail("Invalid base64 in Authorization header"));
        }

        var decoded = Encoding.UTF8.GetString(bytes); // "login:password"

        // ─── 3. Разделяем по первому ':' ──────────────────────────────────
        var separator = decoded.IndexOf(':');
        if (separator < 0)
            return Task.FromResult(AuthenticateResult.Fail("Invalid credentials format"));

        var login = decoded[..separator];
        var password = decoded[(separator + 1)..];

        // ─── 4. Проверяем через UserService ───────────────────────────────
        var userId = _userService.Authenticate(login, password);
        if (userId is null)
            return Task.FromResult(AuthenticateResult.Fail("Invalid login or password"));

        // ─── 5. Строим "паспорт" пользователя ─────────────────────────────
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