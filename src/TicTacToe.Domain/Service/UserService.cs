using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Service;

public class UserService(IUserRepository users) : IUserService
{
    public bool Register(string login, string password)
    {
        if (users.ExistsByLogin(login)) return false;
        users.Register(new User(Guid.NewGuid(), login), password);
        return true;
    }

    public Guid? Authenticate(string login, string password)
    {
        return users.TryAuthenticate(login, password, out var id) ? id : null;
    }

    public User? GetById(Guid id) => users.GetById(id);
}