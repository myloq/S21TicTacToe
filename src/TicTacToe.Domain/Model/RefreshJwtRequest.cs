namespace TicTacToe.Domain.Model;

public class RefreshJwtRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}