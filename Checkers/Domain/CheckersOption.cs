using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Domain;

public class CheckersOption
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    [Range(4, 32)]
    public int GameBoardHeight { get; set; } = 8;
    [Range(4, 32)]
    public int GameBoardWidth { get; set; } = 8;
    public EPlayerType WhitePieces { get; set; } = EPlayerType.Human;
    public EPlayerType BlackPieces { get; set; } = EPlayerType.Human;
    public ICollection<CheckersGame>? CheckersGames { get; set; }

    public override string ToString()
    {
        return $"Name: {Name} Board: {GameBoardHeight}x{GameBoardWidth} WhitePieces: {WhitePieces} BlackPieces: {BlackPieces}";
    }
    
    public void AddCheckersGame(CheckersGame checkersGame)
    {
        CheckersGames?.Add(checkersGame);
    }
    
   public void SetSide(ESide player, EPlayerType playerType)
   {
       switch (player)
       {
           case ESide.White:
               WhitePieces = playerType;
               break;
           case ESide.Black:
               BlackPieces = playerType;
               break;
       }
   }
}