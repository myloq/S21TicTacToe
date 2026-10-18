using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IUserService
{
    bool Register(string login, string password);

    Guid? Authenticate(string login, string password);

    User? GetById(Guid id);
}