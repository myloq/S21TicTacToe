namespace TicTacToe.Domain.Model;

public enum GameStatus
{
    InGame,
    CrossTurn,
    ZeroTurn,
    CrossWin,
    ZeroWin,
    Draw
}

public class GameStatusExtension
{
    public GameStatus GetOpponentStatus(GameStatus status)
    {
        if (status != GameStatus.CrossTurn && status != GameStatus.ZeroTurn) return status;
        return status == GameStatus.CrossTurn ? GameStatus.ZeroTurn : GameStatus.CrossTurn;
    }
}