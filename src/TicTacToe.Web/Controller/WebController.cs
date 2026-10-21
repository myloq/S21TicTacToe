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
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class WebController(IGameService gameService) : ControllerBase
{
    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpPost]
    public IActionResult NewGame([FromBody] CreateGameRequest request)
    {
        bool againstBot = request.OpponentType == "bot";
        return Ok(GameWebMapper.ToWeb(gameService.CreateGame(CurrentUserId, againstBot)));
    }
    
    [HttpGet("available")]
    public IActionResult GetAvailableGames()
    {
        return Ok(gameService.GetAvailableGames(CurrentUserId).Select(GameWebMapper.ToWeb));
    }
    
    [HttpPost("{id:guid}/join")]
    public IActionResult JoinGame(Guid id)
    {
        var game = gameService.JoinGame(CurrentUserId, id);
        return Ok(GameWebMapper.ToWeb(game));
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