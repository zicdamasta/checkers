namespace GameBrain;

public partial class CheckersBrain
{
    // get opponent's color
    public EGamePiece GetOpponentPieceColor()
    {
        return LocalState.NextMoveByBlack ? EGamePiece.White : EGamePiece.Black;
    }
    public EGamePiece GetPlayerPieceColor()
    {
        return LocalState.NextMoveByBlack ? EGamePiece.Black : EGamePiece.White;
    }

    private EGamePiece GetPlayerKingPieceColor()
    {
        return LocalState.NextMoveByBlack ? EGamePiece.BlackKing : EGamePiece.WhiteKing;
    }
    private EGamePiece GetOpponentKingPieceColor()
    {
        return LocalState.NextMoveByBlack ? EGamePiece.WhiteKing : EGamePiece.BlackKing;
    }

    private bool IsCurrentPlayerPiece(EGamePiece? piece)
    {
        return piece == GetPlayerPieceColor() || piece == GetPlayerKingPieceColor();
    }
    
    // is opponent's piece
    private bool IsOpponentPiece(EGamePiece? piece)
    {
        return piece == GetOpponentPieceColor() || piece == GetOpponentKingPieceColor();
    }

    private bool IsNormalPiece(EGamePiece? piece)
    {
        return piece is EGamePiece.Black or EGamePiece.White;
    }

    private bool IsKingPiece(EGamePiece? piece)
    {
        return piece is EGamePiece.BlackKing or EGamePiece.WhiteKing;
    }

    private bool IsFreeTile(Coordinates coordinates)
    {
        return LocalState.GameBoard[coordinates.Row][coordinates.Col] == null;
    }

    private bool NotOutOfBounds(Coordinates coordinates)
    {
        return coordinates.Row >= 0 && 
               coordinates.Row < GameBoardHeight && 
               coordinates.Col >= 0 &&
               coordinates.Col < GameBoardWidth;
    }

    public bool IsGameOver()
    {
        return GetPossibleMoves().Count <= 0;
    }
}