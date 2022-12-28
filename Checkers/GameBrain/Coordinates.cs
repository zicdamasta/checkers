namespace GameBrain;

public readonly struct Coordinates
{
    public int Row { get; } = -1;
    public int Col { get; } = -1;

    public Coordinates(int row, int col)
    {
        Row = row;
        Col = col;
    }
    
    public bool Equals(int row, int col)
    {
        return Row == row && Col == col;
    }
    
}