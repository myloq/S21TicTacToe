using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IGameService
{
    Game CreateGame(Guid userId, bool againstBot);
    Game GetGame(Guid userId, Guid gameId);
    Game MakeMove(Guid userId, Guid gameId, Game incoming);
    IEnumerable<Game> GetAvailableGames(Guid userId);
    Game JoinGame(Guid currentUserId, Guid gameId);
}