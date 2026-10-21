namespace TicTacToe.Web.Model;

public class GameWeb
{
    public Guid Id { get; set; }
    public int[][] Board { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    
    public Guid Player1Id { get; set; }
    public Guid? Player2Id { get; set; }
    public Guid? CurrentPlayerId { get; set; }
    public Guid? WinnerId { get; set; }
    public int Player1Symbol { get; set; }
    public int Player2Symbol { get; set; }
}