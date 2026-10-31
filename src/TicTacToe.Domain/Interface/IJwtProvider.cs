using System.Security.Claims;
using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IJwtProvider
{
    (string AccessToken, string RefreshToken) GenerateTokens(User user);
    bool ValidateToken(string token);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}