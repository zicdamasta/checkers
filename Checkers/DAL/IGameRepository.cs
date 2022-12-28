using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    public string Name { get; set; }
    List<CheckersGame> GetSavedGames();
    
    CheckersGame GetSavedGame(Guid id);
    
    void SaveGame(CheckersGame game);
    
    CheckersGameState SaveState(CheckersGame game, CheckersLocalState state);
    
    void AddStateToSavedGame(Guid checkersGameId, Guid checkersGameStateId);
    
    void DeleteGameStatesByGameId(Guid checkersGameId);
    
    void DeleteGame(Guid id);
    
    void SetWinner(Guid id, string? winner);
    
    void DeleteGameByOptionsId(Guid optionsId);
    
    int CountGamesByOptionsId(Guid optionsId);
}