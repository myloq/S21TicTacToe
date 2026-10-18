namespace TicTacToe.Web.Model;

public class GameWeb
{
    public Guid Id { get; set; }
    public int[][] Board { get; set; } = [];
    public string Status { get; set; } = string.Empty;
}