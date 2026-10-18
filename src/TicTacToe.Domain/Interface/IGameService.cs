using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IGameService
{
    Game CreateGame(Guid userId);
    Game GetGame(Guid userId, Guid gameId);
    
    
    Game MakeMove(Guid userId, Guid gameId, Game incoming);
    
    Game GetBestMove(Game game);
    
    bool ValidateGame(Game game, Game lastState);
    
    GameStatus CheckForWinner(Game game);
}