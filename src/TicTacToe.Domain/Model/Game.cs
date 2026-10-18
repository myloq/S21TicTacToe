namespace TicTacToe.Domain.Model;

public class Game(Guid id, Guid player1Id, Guid? player2Id, GameField field, GameStatus status)
{
    public Guid Id { get; } = id;
    public Guid Player1Id { get; set; } = player1Id;
    public Guid? Player2Id { get; set; } = player2Id;
    public GameField Field { get; } = field;
    public GameStatus Status { get; private set; } = status;

    public void ChangeStatus(GameStatus status)
    {
        Status = status;
    }
}