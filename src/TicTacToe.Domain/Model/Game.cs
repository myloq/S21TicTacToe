namespace TicTacToe.Domain.Model;

public class Game(
    Guid id,
    Guid player1Id,
    Guid? player2Id,
    GameField field,
    GameStatus status,
    Guid? currentPlayerId,
    Guid? winnerId,
    int player1Symbol,
    int player2Symbol)
{
    public Guid Id { get; } = id;
    public Guid Player1Id { get; init; } = player1Id;
    public Guid? Player2Id { get; set; } = player2Id;
    public GameField Field { get; } = field;
    public GameStatus Status { get; private set; } = status;
    public Guid? CurrentPlayerId { get; private set; } = currentPlayerId;
    public Guid? WinnerId { get; set; } = winnerId;
    public int Player1Symbol { get; set; } = player1Symbol;
    public int Player2Symbol { get; set; } = player2Symbol;

    public void ChangeStatus(GameStatus status)
    {
        Status = status;
    }
    
    public void ChangeCurrentPlayerId()
    {
        CurrentPlayerId = CurrentPlayerId == Player1Id ? Player2Id : Player1Id;
    }
    
    public int GetPlayerSymbol(Guid? playerId)
    {
        return playerId == Player1Id ? Player1Symbol : Player2Symbol;
    }
    
    public void SetCurrentPlayerId(Guid? playerId)
    {
        CurrentPlayerId = playerId;
    }
}