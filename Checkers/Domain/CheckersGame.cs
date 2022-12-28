using System.ComponentModel.DataAnnotations;

namespace Domain;

public class CheckersGame
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = default!;

    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime? GameOverAt { get; set; }
    public string? GameWonByPlayer { get; set; }

    [MaxLength(128)]
    public string WhiteName { get; set; } = default!;

    public EPlayerType WhitePieces { get; set; }

    [MaxLength(128)]
    public string BlackName { get; set; } = default!;

    public EPlayerType BlackPieces { get; set; }

    public Guid CheckerOptionId { get; set; }
    public CheckersOption? CheckerOption { get; set; }

    public ICollection<CheckersGameState>? CheckersGameStates { get; set; }
    
    
    public void AddGameState(CheckersGameState checkersGameState)
    {
        CheckersGameStates?.Add(checkersGameState);
    }
    
    // get the last game state
    public CheckersGameState? GetLastGameState()
    {
        return CheckersGameStates?.MaxBy(x => x.CreatedAt);
    }

}