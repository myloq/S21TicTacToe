namespace TicTacToe.Domain.Model;

public class User(Guid id, string login, int wins, int losses, int draws)
{
    public Guid Id { get; } = id;
    public string Login { get; } = login;

    public int Wins { get; set; } = wins;
    public int Losses { get; set; } = losses;
    public int Draws { get; set; } = draws;
}