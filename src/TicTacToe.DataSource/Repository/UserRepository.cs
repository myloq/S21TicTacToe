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
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
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
        return model is null ? null : new User(model.PlayerId, model.Login, model.Wins, model.Losses, model.Draws);
    }

    public void Update(User user)
    {
        var model = db.Users.Find(user.Id)!;
        model.Wins = user.Wins;
        model.Draws = user.Draws;
        model.Losses = user.Losses;
        db.Update(model);
    }

    public IEnumerable<LeaderboardEntry> GetTopPlayers(int count)
    {
        return db.Users
            .OrderByDescending(u => (double)u.Wins / (u.Losses + u.Draws + u.Wins + 1))
            .Take(count)
            .Select(u => new LeaderboardEntry
            {
                Login = u.Login, 
                UserId = u.PlayerId,
                WinRatio = (u.Losses + u.Draws) == 0 ? u.Wins : (double)u.Wins / (u.Losses + u.Draws + u.Wins)
            })
            .ToList();
    }
}