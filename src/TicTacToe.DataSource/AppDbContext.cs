using Microsoft.EntityFrameworkCore;
using TicTacToe.Datasource.Model;

namespace TicTacToe.Datasource;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<GameModel> Games => Set<GameModel>();

    public DbSet<UserModel> Users => Set<UserModel>();
}
