using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Db;

public class GameOptionsRepositoryDb : IGameOptionsRepository
{
    private readonly AppDbContext _dbContext;

    public GameOptionsRepositoryDb(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string Name { get; set; } = "Database";

    public List<CheckersOption> GetGameOptionsList()
    {
        var res = _dbContext.CheckersOption
            .Include(o => o.CheckersGames)
            .ToList();
        return res;
    }

    public CheckersOption GetGameOptions(Guid id)
    {
        return _dbContext.CheckersOption.First(o => o.Id == id);
    }

    public CheckersOption SaveGameOptions(CheckersOption option)
    {
        var newOption = new CheckersOption
        {
            Id = Guid.NewGuid(),
            Name = option.Name,
            GameBoardHeight = option.GameBoardHeight,
            GameBoardWidth = option.GameBoardWidth,
            WhitePieces = option.WhitePieces,
            BlackPieces = option.BlackPieces,
        };
        
        _dbContext.CheckersOption.Add(newOption);
        _dbContext.SaveChanges();
        return newOption;
    }

    public void OptionsNameIsUnique(CheckersOption option)
    {
        var res = _dbContext.CheckersOption
            .FirstOrDefault(o => o.Name == option.Name);
        if (res != null)
        {
            throw new Exception("Name must be unique");
        }
    }

    public void DeleteGameOptions(Guid id)
    {
        var options = GetGameOptions(id);
        _dbContext.CheckersOption.Remove(options);
        _dbContext.SaveChanges();
    }
}