using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IAuthService
{
    JwtResponse Authenticate(JwtRequest request);
    JwtResponse UpdateAccessToken(RefreshJwtRequest request);
    JwtResponse UpdateRefreshToken(RefreshJwtRequest request);
}