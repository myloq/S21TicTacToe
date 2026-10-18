using TicTacToe.Datasource.Model;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Repository;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public bool ExistsByLogin(string login) => db.Users.Any(u => u.Login == login);

    public void Register(User user, string password)
    {
        db.Users.Add(new UserModel()
        {
            PlayerId = user.Id,
            Login = user.Login,
            Password = password
        });
        db.SaveChanges();
    }

    public bool TryAuthenticate(string login, string password, out Guid userId)
    {
        var user = db.Users.FirstOrDefault(u => u.Login == login);
        if (user is null || user.Password != password)
        {
            userId = Guid.Empty;
            return false;
        }
        userId = user.PlayerId;
        return true;
    }

    public User? GetById(Guid id)
    {
        var model = db.Users.Find(id);
        return model is null ? null : new User(model.PlayerId, model.Login);
    }
}