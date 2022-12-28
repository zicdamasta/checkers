using Domain;
using GameBrain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Db;

public class GameRepositoryDatabase : IGameRepository
{
    private readonly AppDbContext _dbContext;

    public GameRepositoryDatabase(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public string Name { get; set; } = "Database";
    public List<CheckersGame> GetSavedGames()
    {
        return _dbContext.CheckersGame
            .Include(g => g.CheckerOption)
            .Include(g => g.CheckersGameStates)
            .ToList();
    }

    public CheckersGame GetSavedGame(Guid id)
    {
        return _dbContext.CheckersGame
            .Include(g => g.CheckerOption)
            .Include(g => g.CheckersGameStates)
            .First(g => g.Id == id);
    }

    public void SaveGame(CheckersGame game)
    {
        _dbContext.CheckersGame.Add(game);
        _dbContext.SaveChanges();
    }

    public CheckersGameState SaveState(CheckersGame game, CheckersLocalState state)
    {
        var stateAsJson = System.Text.Json.JsonSerializer.Serialize(state);
        var gameState = new CheckersGameState
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            SerializedGameState = stateAsJson,
            CheckersGameId = game.Id,
            CheckersGame = game,
        };
        _dbContext.CheckersGameState.Add(gameState);
        _dbContext.SaveChanges();
        return gameState;
    }

    public void AddStateToSavedGame(Guid checkersGameId, Guid checkersGameStateId)
    {
        // handled by EF Core
    }

    public void DeleteGameStatesByGameId(Guid checkersGameId)
    {
        var gameStates = _dbContext.CheckersGameState.Where(g => g.CheckersGameId == checkersGameId);
        _dbContext.CheckersGameState.RemoveRange(gameStates);
        _dbContext.SaveChanges();
    }

    public void DeleteGame(Guid id)
    {
        var game = _dbContext.CheckersGame.First(g => g.Id == id);
        _dbContext.CheckersGame.Remove(game);
        _dbContext.SaveChanges();
    }

    public void SetWinner(Guid id, string? winner)
    {
        // check if GameWonByPlayer is not set
        var game = _dbContext.CheckersGame.First(g => g.Id == id);
        if (game.GameWonByPlayer == null)
        {
            game.GameWonByPlayer = winner;
            game.GameOverAt = DateTime.Now;
            _dbContext.SaveChanges();
        }
    }

    public void DeleteGameByOptionsId(Guid optionsId)
    {
        // managed by EF Core
    }

    public int CountGamesByOptionsId(Guid optionsId)
    {
        return _dbContext.CheckersGame.Count(g => g.CheckerOptionId == optionsId);
    }
}