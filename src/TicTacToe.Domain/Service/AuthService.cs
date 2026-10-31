using System.Security.Claims;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Service;

public class AuthService(IUserService userService, IJwtProvider jwtProvider) : IAuthService
{
    public JwtResponse Authenticate(JwtRequest request)
    {
        var userId = userService.Authenticate(new SignUpRequest()
        {
            Login = request.Login,
            Password = request.Password
        });
        
        if (userId is null) throw new UnauthorizedException("Invalid login or password");
        var user = userService.GetById(userId.Value);
        if (user is null) throw new UnauthorizedException("Invalid user");
        
        var tokens = jwtProvider.GenerateTokens(user);
        
        return new JwtResponse() {AccessToken = tokens.AccessToken, RefreshToken = tokens.RefreshToken};
    }

    public JwtResponse UpdateAccessToken(RefreshJwtRequest request)
    {
        var isValidRequest = jwtProvider.ValidateToken(request.RefreshToken);
        if (!isValidRequest) throw new UnauthorizedException("Invalid refresh token");

        var principal = jwtProvider.GetPrincipalFromToken(request.RefreshToken);
        if (principal is null) throw new UnauthorizedException("Invalid refresh token");

        var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdString is null) throw new UnauthorizedException("Invalid user");
        var userId = Guid.Parse(userIdString);

        var user = userService.GetById(userId);
        if (user is null) throw new UnauthorizedException("Invalid user");

        var tokens = jwtProvider.GenerateTokens(user);
        return new JwtResponse() { AccessToken = tokens.AccessToken, RefreshToken = tokens.RefreshToken };
    }

    public JwtResponse UpdateRefreshToken(RefreshJwtRequest request)
    {
        return UpdateAccessToken(request);
    }
}