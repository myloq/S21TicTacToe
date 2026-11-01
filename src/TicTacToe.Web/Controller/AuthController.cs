using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;
using TicTacToe.Web.Model;

namespace TicTacToe.Web.Controller;

[ApiController]
[Route("auth")]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] SignUpRequest request)
    {
        userService.Register(request);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] JwtRequest request)
    {
        var result = authService.Authenticate(request);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh/access")]
    public IActionResult RefreshAccess([FromBody] RefreshJwtRequest request)
    {
        var result = authService.UpdateAccessToken(request);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshJwtRequest request)
    {
        var result = authService.UpdateRefreshToken(request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("users/{id:guid}")]
    public IActionResult GetUser(Guid id)
    {
        var user = userService.GetById(id);
        if (user is null) return NotFound(new { message = "User not found" });
        return Ok(new UserWeb { Id = user.Id, Login = user.Login });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdString is null) return Unauthorized();
        var userId = Guid.Parse(userIdString);

        var user = userService.GetById(userId);
        if (user is null) return NotFound(new { message = "User not found" });
        return Ok(new UserWeb { Id = user.Id, Login = user.Login });
    }
}