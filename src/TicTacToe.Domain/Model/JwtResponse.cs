namespace TicTacToe.Domain.Model;

public class JwtResponse
{
    public string Type { get; set; } = "Bearer";
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}