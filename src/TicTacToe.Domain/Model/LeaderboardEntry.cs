namespace TicTacToe.Domain.Model;

public class LeaderboardEntry
{
    public Guid UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public double WinRatio { get; set; }
}