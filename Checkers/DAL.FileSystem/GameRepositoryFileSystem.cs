using System.Text.Json;
using Domain;

namespace DAL.FileSystem;

public class GameRepositoryFileSystem : IGameRepository
{
    private const string FileExtension = "json";
    private readonly string _gamesDirectory = "." + Path.DirectorySeparatorChar + "games";
    private readonly string _statesDirectory = "." + Path.DirectorySeparatorChar + "states";
    public string Name { get; set; } = "FileSystem";
    
    public List<CheckersGame> GetSavedGames()
    {
        Helpers.CheckOrCreateDirectory(_gamesDirectory);
        var games = new List<CheckersGame>();
        
        foreach (var fileName in Directory.GetFileSystemEntries(_gamesDirectory, "*." + FileExtension))
        {
            var json = File.ReadAllText(fileName);
            var game = JsonSerializer.Deserialize<CheckersGame>(json);
            if (game?.Name != null)
            {
                games.Add(game);
            }
        }
        
        return games;
    }

    public CheckersGame GetSavedGame(Guid gameId)
    {
        var fileContent = File.ReadAllText(Helpers.GetFileName(_gamesDirectory, gameId.ToString(), FileExtension));
        var game = JsonSerializer.Deserialize<CheckersGame>(fileContent);
        if (game == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }

        return game;
    }
    
    // get saved state
    public CheckersGameState GetSavedState(Guid stateId)
    {
        var fileContent = File.ReadAllText(Helpers.GetFileName(_statesDirectory, stateId.ToString(), FileExtension));
        var state = JsonSerializer.Deserialize<CheckersGameState>(fileContent);
        if (state == null)
        {
            throw new NullReferenceException($"Could not deserialize: {fileContent}");
        }

        return state;
    }

    public void SaveGame(CheckersGame game)
    {
        if (game.Id == Guid.Empty)
        {
            game.Id = Guid.NewGuid();
        }
        Helpers.CheckOrCreateDirectory(_gamesDirectory);

        var fileContent = JsonSerializer.Serialize(game);

        File.WriteAllText(Helpers.GetFileName(_gamesDirectory, game.Id.ToString(), FileExtension), fileContent);
    }

    public CheckersGameState SaveState(CheckersGame game, CheckersLocalState state)

    {
        Helpers.CheckOrCreateDirectory(_statesDirectory);
        var stateId = Guid.NewGuid();
        
        var stateAsJson = System.Text.Json.JsonSerializer.Serialize(state);
        var gameState = new CheckersGameState
        {
            Id = stateId,
            CreatedAt = DateTime.Now,
            SerializedGameState = stateAsJson,
            CheckersGameId = game.Id,
            CheckersGame = game,
        };
        var fileContent = JsonSerializer.Serialize(gameState);
        File.WriteAllText(Helpers.GetFileName(_statesDirectory, gameState.Id.ToString(), FileExtension), fileContent);
        return gameState;
    }

    public void AddStateToSavedGame(Guid checkersGameId, Guid checkersGameStateId)
    { 
        var game = GetSavedGame(checkersGameId);
        var state = GetSavedState(checkersGameStateId);
        game.CheckersGameStates?.Add(state);
        DeleteGame(checkersGameId);
        SaveGame(game);
    }

    private void DeleteState(Guid stateId)
    {
        File.Delete(Helpers.GetFileName(_statesDirectory, stateId.ToString(), FileExtension));
    }
    
    public void DeleteGameStatesByGameId(Guid gameId)
    {
        var game = GetSavedGame(gameId);
        if (game.CheckersGameStates != null)
            foreach (var state in game.CheckersGameStates)
            {
                DeleteState(state.Id);
            }
    }
    
    public void DeleteGame(Guid id)
    {
        File.Delete(Helpers.GetFileName(_gamesDirectory, id.ToString(), FileExtension));
    }

    public void SetWinner(Guid id, string? winner)
    {
        var game = GetSavedGame(id);
        if (game.GameWonByPlayer == null)
        {
            game.GameWonByPlayer = winner;
            game.GameOverAt = DateTime.Now;
            DeleteGame(id);
            SaveGame(game);
        }
    }

    public void DeleteGameByOptionsId(Guid optionsId)
    {
        var games = GetSavedGames();
        foreach (var game in games.Where(game => game.CheckerOptionId == optionsId))
        {
            DeleteGame(game.Id);
        }
    }

    public int CountGamesByOptionsId(Guid optionsId)
    {
        var games = GetSavedGames();
        return games.Count(game => game.CheckerOptionId == optionsId);
    }
}