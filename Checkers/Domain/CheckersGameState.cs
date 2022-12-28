using GameBrain;

namespace Domain;

public class CheckersGameState
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public string SerializedGameState { get; set; } = default!;

    public Guid CheckersGameId { get; set; }
    public CheckersGame? CheckersGame { get; set; }
}