using TicTacToe.Domain.Model;
using TicTacToe.Web.Model;

namespace TicTacToe.Web.Mapper;

public static class GameWebMapper
{
    public static GameWeb ToWeb(Game game) => new()
    {
        Id = game.Id,
        Board = ToJaggedArray(game.Field.Board),
        Status = game.Status.ToString(),
        
        Player1Id = game.Player1Id,
        Player2Id = game.Player2Id,
        CurrentPlayerId = game.CurrentPlayerId,
        WinnerId = game.WinnerId,
        Player1Symbol = game.Player1Symbol,
        Player2Symbol = game.Player2Symbol
    };

    public static Game ToDomain(GameWeb web)
    {
        var s = Enum.TryParse<GameStatus>(web.Status, out var status) ? status : GameStatus.InProgress;
        return new Game(web.Id, Guid.Empty, null, new GameField(ToRectArray(web.Board)), s, null, null, 0, 0);
    }

    private static int[][] ToJaggedArray(int[,] array)
    {
        var result = new int[3][];
        for (int i = 0; i < 3; i++)
        {
            result[i] = new int[3];
            for (int j = 0; j < 3; j++) result[i][j] = array[i, j];
        }
        return result;
    }

    private static int[,] ToRectArray(int[][] array)
    {
        var result = new int[3,3];
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++) result[i,j] = array[i][j];
        return result;
    }
}