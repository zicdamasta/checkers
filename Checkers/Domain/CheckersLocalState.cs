using GameBrain;

namespace Domain;

public class CheckersLocalState
{
    public EGamePiece?[][] GameBoard { get; set; } = default!;
    public bool NextMoveByBlack { get; set; } = false;
    
    // create equals method if gameboard and nextmove is black is the same
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
    
        CheckersLocalState other = (CheckersLocalState)obj;
        if (this.NextMoveByBlack != other.NextMoveByBlack)
        {
            return false;
        }
    
        for (int i = 0; i < GameBoard.Length; i++)
        {
            for (int j = 0; j < GameBoard[i].Length; j++)
            {
                if (this.GameBoard[i][j] != other.GameBoard[i][j])
                {
                    return false;
                }
            }
        }
    
        return true;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(GameBoard, NextMoveByBlack);
    }
}