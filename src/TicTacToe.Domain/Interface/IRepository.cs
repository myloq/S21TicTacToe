using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IRepository
{
    void Save(Game game);
    Game? GetGame(Guid id);
    IEnumerable<Game> GetAvailableGames();
}