using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Domain.Interface;
using TicTacToe.Web.Mapper;
using TicTacToe.Web.Model;

namespace TicTacToe.Web.Controller;

[ApiController]
[Route("game")]
[Authorize]
public class WebController(IGameService gameService) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
    
    [HttpPost]
    public IActionResult NewGame()
    {
        return Ok(GameWebMapper.ToWeb(gameService.CreateGame(CurrentUserId)));
    }
    
    [HttpGet("{id:guid}")]
    public IActionResult Get(Guid id)
    {
        var game = gameService.GetGame(CurrentUserId, id);
        return Ok(GameWebMapper.ToWeb(game));
    }

    [HttpPost("{id:guid}")]
    public IActionResult Move(Guid id, [FromBody]GameWeb request)
    {
        var incoming = GameWebMapper.ToDomain(request);
        var result = gameService.MakeMove(CurrentUserId, id, incoming);
        return Ok(GameWebMapper.ToWeb(result));
    }
}