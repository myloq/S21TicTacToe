using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;
using TicTacToe.Web.Authenticator;
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
        userService.Register(request);
        return Ok();
    }

    [Authorize(AuthenticationSchemes = UserAuthenticator.SchemeName)]
    [HttpPost("login")]
    public IActionResult Login()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return id is null ? Unauthorized() : Ok(new { userId = id });
    }

    [Authorize]
    [HttpGet("users/{id:guid}")]
    public IActionResult GetUser(Guid id, [FromServices] IUserService userService)
    {
        var user = userService.GetById(id);
        if (user is null) return NotFound(new {message = "User not found"});
        return Ok(new UserWeb { Id = user.Id, Login = user.Login });
    }
}