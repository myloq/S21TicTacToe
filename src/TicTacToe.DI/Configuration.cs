using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Datasource;
using TicTacToe.Datasource.Repository;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Service;

namespace TicTacToe.DI;

public static class Configuration
{
    public static IServiceCollection ConfigureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IGameService, GameService>();

        return services;
    }
}