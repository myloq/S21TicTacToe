using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IUserRepository
{
    bool ExistsByLogin(string login);
    void Register(User user, string password);
    public bool TryAuthenticate(string login, string password, out Guid userId);
    User? GetById(Guid id);
}
