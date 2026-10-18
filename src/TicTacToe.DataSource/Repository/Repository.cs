using TicTacToe.Datasource.Mapper;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Repository;

public class Repository(AppDbContext db) : IRepository
{
    public void Save(Game game)
    {
        var model = DataModelMapper.ToData(game);
        var existing = db.Games.Find(game.Id);

        if (existing is null)
        {
            db.Games.Add(model);
        }
        else
        {
            existing.Board = model.Board;
            existing.Status = model.Status;
            existing.Player2Id = game.Player2Id;
        }

        db.SaveChanges();
    }

    public Game? GetGame(Guid id)
    {
        var model = db.Games.Find(id);
        return model is null ? null : DataModelMapper.ToGame(model);
    }
}