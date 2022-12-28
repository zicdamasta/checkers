using GameBrain;
using Spectre.Console;

namespace ConsoleUI;

public class UserInterface

{
    private static int _selectedRow;
    private static int _selectedCol;

    public UserInterface()
    {
        _selectedRow = 0;
        _selectedCol = 0;
    }

    public Move GetMove(EGamePiece?[][] board, EGamePiece player)
    {
        var rows = board.GetLength(0);
        var cols = board[0].GetLength(0);

        var startCoordinates = new Coordinates(-1, -1);
        var endCoordinates = new Coordinates(-1, -1);

        var pause = false;
        
        do
        {
            DrawBoard(board, rows, cols, startCoordinates);
            Console.WriteLine(player);
            
            // write the selected coordinates
            if (startCoordinates.Row != -1)
            {
                Console.WriteLine($"Selected: {startCoordinates.Row}, {startCoordinates.Col}");
            }

            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    _selectedRow = Math.Max(0, _selectedRow - 1);
                    break;
                case ConsoleKey.DownArrow:
                    _selectedRow = Math.Min(rows - 1, _selectedRow + 1);
                    break;
                case ConsoleKey.LeftArrow:
                    _selectedCol = Math.Max(0, _selectedCol - 1);
                    break;
                case ConsoleKey.RightArrow:
                    _selectedCol = Math.Min(cols - 1, _selectedCol + 1);
                    break;
                case ConsoleKey.Backspace:
                    pause = true;
                    break;
                case ConsoleKey.Enter:
                    if (startCoordinates.Equals(-1, -1))
                    {
                        startCoordinates = new Coordinates(_selectedRow, _selectedCol);
                    }
                    else
                    {
                        endCoordinates = new Coordinates(_selectedRow, _selectedCol);
                    }
                    break;
            }
        } while (!pause && !MoveIsCompleted(startCoordinates, endCoordinates));
        return pause ? new Move(new Coordinates(-1, -1), new Coordinates(-1, -1)) : new Move(startCoordinates, endCoordinates);
    }
    private static bool MoveIsCompleted(Coordinates startCoordinates, Coordinates endCoordinates)
    {
        return !startCoordinates.Equals(-1, -1) && !endCoordinates.Equals(-1, -1);
    }

    public static void DrawBoard(EGamePiece?[][] board, int rows, int cols, Coordinates confirmedCoordinates)
    {
        const string darkTileColor = "chartreuse4";
        const string lightTileColor = "cornsilk1";
        const string piece = "⬤";
        const string kingPiece = "Ⓚ";
        const string lightPieceColor = "grey100";
        const string darkPieceColor = "grey0";

        var chessBoard = "";

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                var tileColor = IsSelected((i, j), (_selectedRow, _selectedCol)) ? "red" :
                    (i + j) % 2 == 0 ? lightTileColor : darkTileColor;
                
                var effect = confirmedCoordinates.Equals(i, j) ? "blink" : "default";

                chessBoard += board[i][j] switch
                {
                    EGamePiece.Black => $"[{effect} {darkPieceColor} on {tileColor}] {piece} [/]",
                    EGamePiece.BlackKing => $"[{effect} {darkPieceColor} on {tileColor}] {kingPiece} [/]",
                    EGamePiece.White => $"[{effect} {lightPieceColor} on {tileColor}] {piece} [/]",
                    EGamePiece.WhiteKing => $"[{effect} {lightPieceColor} on {tileColor}] {kingPiece} [/]",
                    _ => $"[{effect} on {tileColor}]   [/]"
                };
            }

            chessBoard += "\n";
        }

        Console.Clear();
        AnsiConsole.Write(new Markup(chessBoard));
    }
    
    private static bool IsSelected((int, int) currentCoordinates, (int, int) selectedCoordinates)
    {
        return currentCoordinates == selectedCoordinates;
    }
    
    
}