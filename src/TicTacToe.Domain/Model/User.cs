namespace TicTacToe.Domain.Model;

public class User(Guid id, string login)
{
    public Guid Id { get; } = id;
    public string Login { get; } = login;
}