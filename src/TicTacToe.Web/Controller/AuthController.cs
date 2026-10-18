using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Interface;
using TicTacToe.Web.Model;

namespace TicTacToe.Web.Controller;

[ApiController]
[Route("auth")]
public class AuthController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] SignUpRequest request)
    {
        var ok = userService.Register(request.Login, request.Password);
        return ok ? Ok() : Conflict(new { message = "Username is already taken." });
    }

    [Authorize(AuthenticationSchemes = UserController.SchemeName)]
    [HttpPost("login")]
    public IActionResult Login()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return id is null ? Unauthorized() : Ok(new { userId = id });
    }
}