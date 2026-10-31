namespace TicTacToe.Domain.Model;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenLifetimeMinutes { get; set; }
    public int RefreshTokenLifetimeDays { get; set; }
}