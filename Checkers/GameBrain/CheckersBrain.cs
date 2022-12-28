using System.Diagnostics.CodeAnalysis;
using Domain;

namespace GameBrain;

public partial class CheckersBrain
{
    public int GameBoardHeight { get; }
    public int GameBoardWidth { get; }

    public readonly CheckersLocalState LocalState;


    public CheckersBrain(CheckersOption option, CheckersLocalState? gameLocalState)

    {
        var gameBoardWidth = option.GameBoardWidth;
        var gameBoardHeight = option.GameBoardHeight;

        if (gameBoardWidth < 0 || gameBoardHeight < 0)
        {
            throw new ArgumentException("Board size too small");
        }

        if (gameBoardWidth > 32 || gameBoardHeight > 32)
        {
            throw new ArgumentException("Board size too large");
        }

        if (gameBoardHeight % 2 != 0)
        {
            throw new ArgumentException("Board height must be even");
        }

        if (gameLocalState is null)
        {
            LocalState = new CheckersLocalState
            {
                GameBoard = new EGamePiece?[gameBoardHeight][]
            };
            InitializeGameBoard(gameBoardHeight, gameBoardWidth);
        }
        else
        {
            LocalState = gameLocalState;
        }
        

        
        GameBoardHeight = gameBoardHeight;
        GameBoardWidth = gameBoardWidth;
    }

    // populate board with pieces
    private void InitializeGameBoard(int boardHeight, int boardWidth)
    {
        var piecesRows = boardHeight / 2 - 1;
        for (var i = 0; i < boardHeight; i++)
        {
            LocalState.GameBoard[i] = new EGamePiece?[boardWidth];
            for (var j = 0; j < boardWidth; j++)
            {
                if (i < piecesRows && (i + j) % 2 == 1)
                {
                    LocalState.GameBoard[i][j] = EGamePiece.Black;
                }
                else if (i > piecesRows + 1 && (i + j) % 2 == 1)
                {
                    LocalState.GameBoard[i][j] = EGamePiece.White;
                }
                else
                {
                    LocalState.GameBoard[i][j] = null;
                }
            }
        }
    }

    
    public CheckersLocalState GetLocalState()
    {
        return LocalState;
    }
    
    public bool MakeMove(Move move)
    {
        if (!IsMoveValid(move)) return false;


        MovePiece(move);

        var jumpedOverPieces = JumpedOverPieces(move);

        if (jumpedOverPieces)
        {
            RemoveEatenPieces(move);
        }

        var isKingMade = MakeKingIfNeeded(move.To);
        
        // can eat again
        if (!isKingMade && jumpedOverPieces && CanEatPiece(move.To)) return true;
        
        ToggleNextMove();
        return true;
    }

    private void ToggleNextMove()
    {
        LocalState.NextMoveByBlack = !LocalState.NextMoveByBlack;
    }
    

    private bool MakeKingIfNeeded(Coordinates coordinates)
    {
        var row = coordinates.Row;
        var col = coordinates.Col;

        var piece = LocalState.GameBoard[row][col];

        var isKingMade = false;

        switch (piece)
        {
            case EGamePiece.White when row == 0:
                LocalState.GameBoard[row][col] = EGamePiece.WhiteKing;
                isKingMade = true;
                break;
            case EGamePiece.Black when row == GameBoardHeight - 1:
                LocalState.GameBoard[row][col] = EGamePiece.BlackKing;
                isKingMade = true;
                break;
            default:
                LocalState.GameBoard[row][col] = LocalState.GameBoard[row][col];
                break;
        }

        return isKingMade;
    }

    private bool JumpedOverPieces(Move move)
    {
        // Get the coordinates between the two positions
        var coordinatesBetween = GetCoordinatesBetween(move);

        // Check if any of the intermediate positions contain a piece
        return coordinatesBetween.Any(coordinates => LocalState.GameBoard[coordinates.Row][coordinates.Col] != null);
    }

    private void MovePiece(Move move)
    {
        var fromRow = move.From.Row;
        var fromCol = move.From.Col;
        var toRow = move.To.Row;
        var toCol = move.To.Col;

        LocalState.GameBoard[toRow][toCol] = LocalState.GameBoard[fromRow][fromCol];
        LocalState.GameBoard[fromRow][fromCol] = null;
    }

    private void RemoveEatenPieces(Move move)
    {
        var fromRow = move.From.Row;
        var fromCol = move.From.Col;
        var toRow = move.To.Row;
        var toCol = move.To.Col;

        // Calculate the difference between the two positions in each dimension
        var xDiff = toRow - fromRow;
        var yDiff = toCol - fromCol;

        // Calculate the step to take in each dimension
        var xStep = xDiff / Math.Abs(xDiff);
        var yStep = yDiff / Math.Abs(yDiff);

        // Start at the position immediately after the starting position
        var x = fromRow + xStep;
        var y = fromCol + yStep;

        // Iterate through the intermediate positions, removing any pieces that are found
        while (x != toRow && y != toCol)
        {
            LocalState.GameBoard[x][y] = null;
            x += xStep;
            y += yStep;
        }
    }


    private bool IsMoveValid(Move move)
    {
        var fromRow = move.From.Row;
        var fromCol = move.From.Col;
        var toRow = move.To.Row;
        var toCol = move.To.Col;

        // dont allow moving to the same place
        if (fromRow == toRow && fromCol == toCol) return false;

        //check that starting move coordinates are not off the board
        if (fromRow < 0 || fromRow >= GameBoardHeight || fromCol < 0 || fromCol >= GameBoardWidth)
        {
            return false;
        }

        //check that ending move coordinates are not off the board
        if (toRow < 0 || toRow >= GameBoardHeight || toCol < 0 || toCol >= GameBoardWidth)
        {
            return false;
        }

        // check for forced eat
        if (IsForcedToEat())
        {
            // check if piece is eating
            if (!CanEatPiece(move.From))
            {
                return false;
            }

            // if GetPiecesBetween is empty, it means that the piece is not eating
            if (GetCoordinatesBetween(move).Count == 0)
            {
                return false;
            }
        }

        // dont allow to move empty space
        if (LocalState.GameBoard[fromRow][fromCol] == null)
        {
            return false;
        }

        // dont allow moving to a place that is not empty
        if (LocalState.GameBoard[toRow][toCol] != null)
        {
            return false;
        }


        if (!IsCurrentPlayerPiece(LocalState.GameBoard[fromRow][fromCol]))
        {
            return false;
        }

        // check if move is diagonal
        if (Math.Abs(fromRow - toRow) != Math.Abs(fromCol - toCol))
        {
            return false;
        }

        if (IsNormalPiece(LocalState.GameBoard[fromRow][fromCol]))
        {
            // if backwards move for black and not eating piece
            if (LocalState.NextMoveByBlack && fromRow > toRow && !CanEatPiece(move.From))
            {
                return false;
            }

            // if backwards move for white and not eating piece
            if (!LocalState.NextMoveByBlack && fromRow < toRow && !CanEatPiece(move.From))
            {
                return false;
            }

            // if move is not one step
            if (Math.Abs(fromRow - toRow) != 1)
            {
                // if move is not two steps
                if (Math.Abs(fromRow - toRow) != 2)
                {
                    return false;
                }

                // if there is no piece to jump over
                if (LocalState.GameBoard[(fromRow + toRow) / 2][(fromCol + toCol) / 2] == null)
                {
                    return false;
                }

                // if piece to jump over is of the same color
                if (IsCurrentPlayerPiece(LocalState.GameBoard[(fromRow + toRow) / 2][(fromCol + toCol) / 2]))
                {
                    return false;
                }
            }
        }

        if (IsKingPiece(LocalState.GameBoard[fromRow][fromCol]))
        {
            // get all pieces between start and end
            var piecesBetween = GetCoordinatesBetween(move);
            // if there are pieces between start and end
            if (piecesBetween.Count > 0)
            {
                // if there is more than one piece between start and end
                if (piecesBetween.Count > 1)
                {
                    return false;
                }

                // if piece between is of the same color
                var piece = LocalState.GameBoard[piecesBetween[0].Row][piecesBetween[0].Col];
                if (IsCurrentPlayerPiece(piece))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Get coordinates of all pieces between start and end

    private List<Coordinates> GetCoordinatesBetween(Move move)
    {
        var coordinatesBetween = new List<Coordinates>();

        var fromRow = move.From.Row;
        var fromCol = move.From.Col;
        var toRow = move.To.Row;
        var toCol = move.To.Col;

        // if same coordinates
        if (fromRow == toRow && fromCol == toCol) return coordinatesBetween;

        // if move is not diagonal
        if (Math.Abs(fromRow - toRow) != Math.Abs(fromCol - toCol)) return coordinatesBetween;

        // get all pieces in diagonal between start and end
        var rowDirection = fromRow < toRow ? 1 : -1;
        var colDirection = fromCol < toCol ? 1 : -1;
        var row = fromRow + rowDirection;
        var col = fromCol + colDirection;
        while (row != toRow && col != toCol)
        {
            if (LocalState.GameBoard[row][col] != null)
            {
                coordinatesBetween.Add(new Coordinates(row, col));
            }

            row += rowDirection;
            col += colDirection;
        }

        return coordinatesBetween;
    }


    // check for all player pieces and check if is forced to eat
    private bool IsForcedToEat()
    {
        for (var row = 0; row < GameBoardHeight; row++)
        {
            for (var col = 0; col < GameBoardWidth; col++)
            {
                if (IsCurrentPlayerPiece(LocalState.GameBoard[row][col]) && CanEatPiece(new Coordinates(row, col)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    //  check surrounding of piece to see if it can eat another piece
    private bool CanEatPiece(Coordinates coordinates)
    {
        var row = coordinates.Row;
        var col = coordinates.Col;

        var piece = LocalState.GameBoard[row][col];
        if (piece == null)
        {
            return false;
        }

        bool IsEatablePieceBetween(IReadOnlyList<Coordinates> piecesBetween)
        {
            if (piecesBetween.Count != 1) return false;
            var firstPiece = LocalState.GameBoard[piecesBetween[0].Row][piecesBetween[0].Col];
            {
                return !IsCurrentPlayerPiece(firstPiece);
            }
        }

        var loopDepth = IsKingPiece(piece) ? GameBoardHeight : 3;
        for (var i = 2; i < loopDepth; i++)
        {
            var destinations = new List<Coordinates>
            {
                new(row + i, col - i), // down left
                new(row + i, col + i), // down right
                new(row - i, col - i), // up left
                new(row - i, col + i) // up right
            };

            foreach (var destination in destinations)
            {
                if (NotOutOfBounds(destination) && IsFreeTile(destination))
                {
                    var piecesBetween = GetCoordinatesBetween(new Move(coordinates, destination));
                    if (IsEatablePieceBetween(piecesBetween))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public EGamePiece?[][] GetBoard()
    {
        var jsonStr = System.Text.Json.JsonSerializer.Serialize(LocalState.GameBoard);
        return System.Text.Json.JsonSerializer.Deserialize<EGamePiece?[][]>(jsonStr)!;
    }


    // scan board for pieces of current player, return map of pieces where key is piece and value is list of possible moves coordinates
    public Dictionary<Coordinates, List<Coordinates>> GetPossibleMoves()
    {
        var possibleMoves = new Dictionary<Coordinates, List<Coordinates>>();

        for (var row = 0; row < GameBoardHeight; row++)
        {
            for (var col = 0; col < GameBoardWidth; col++)
            {
                if (IsCurrentPlayerPiece(LocalState.GameBoard[row][col]))
                {
                    var pieceCoordinates = new Coordinates(row, col);
                    var destinations = GetPossibleDestinationsForPiece(pieceCoordinates);
                    if (destinations.Count > 0)
                    {
                        possibleMoves.Add(pieceCoordinates, destinations);
                    }
                }
            }
        }

        return possibleMoves;
    }
    
    private List<Coordinates> GetPossibleDestinationsForPiece(Coordinates coordinates)
    {
        var destinations = new List<Coordinates>();
        var row = coordinates.Row;
        var col = coordinates.Col;

        var loopDepth = IsKingPiece(LocalState.GameBoard[row][col]) ? GameBoardHeight : 3;
        for (var i = 1; i < loopDepth; i++)
        {
            var possibleDestinations = new List<Coordinates>
            {
                new(row + i, col - i), // down left
                new(row + i, col + i), // down right
                new(row - i, col - i), // up left
                new(row - i, col + i) // up right
            };

            foreach (var destination in possibleDestinations)
            {
                if (IsMoveValid(new Move(coordinates, destination)))
                {
                    destinations.Add(destination);
                }
            }
        }

        return destinations;
    }

    public CheckersBrain CopyGameState()
    {
        var copy = new CheckersBrain(new CheckersOption
            { GameBoardWidth = GameBoardWidth, GameBoardHeight = GameBoardHeight }, null);

        copy.LocalState.GameBoard = new EGamePiece?[GameBoardHeight][];
        for (var i = 0; i < GameBoardHeight; i++)
        {
            copy.LocalState.GameBoard[i] = new EGamePiece?[GameBoardWidth];
            for (var j = 0; j < GameBoardWidth; j++)
            {
                copy.LocalState.GameBoard[i][j] = LocalState.GameBoard[i][j];
            }
        }

        copy.LocalState.NextMoveByBlack = LocalState.NextMoveByBlack;

        return copy;
    }

    public int GetScore()
    {
        var score = 0;
        for (var row = 0; row < GameBoardHeight; row++)
        {
            for (var col = 0; col < GameBoardWidth; col++)
            {
                switch (LocalState.GameBoard[row][col])
                {
                    case EGamePiece.White:
                        score++;
                        break;
                    case EGamePiece.WhiteKing:
                        score += 2;
                        break;
                    case EGamePiece.Black:
                        score--;
                        break;
                    case EGamePiece.BlackKing:
                        score -= 2;
                        break;
                }
            }
        }

        return score;
    }
    
    public EGamePiece? GetWinner()
    {
        if (IsGameOver())
        {
            return LocalState.NextMoveByBlack ? EGamePiece.White : EGamePiece.Black;
        }

        return null;
    }
}