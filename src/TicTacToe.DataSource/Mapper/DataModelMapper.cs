using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Mapper;

public static class DataModelMapper
{
    public static GameModel ToData(Game game) => new()
    {
        GameId = game.Id,
        Player1Id = game.Player1Id,
        Player2Id = game.Player2Id,
        Board = (int[,])game.Field.Board.Clone(),
        Status = (int)game.Status
    };

    public static Game ToGame(GameModel model) =>
        new(model.GameId, model.Player1Id, model.Player2Id, new GameField(model.Board), (GameStatus)model.Status);
}