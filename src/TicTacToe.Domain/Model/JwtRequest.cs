namespace TicTacToe.Domain.Model;

public class JwtRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}