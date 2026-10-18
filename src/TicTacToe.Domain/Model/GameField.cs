namespace TicTacToe.Domain.Model;

public class GameField(int[,] board)
{
    public int[,]  Board { get; } = (int[,])board.Clone();
}
