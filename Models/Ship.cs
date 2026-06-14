namespace Battleships.Models;

public class Ship
{
    public int Size { get; }
    private readonly int Row;
    private readonly int Col;
    private readonly bool IsHorizontal;

    public Ship(int size, int row, int col, bool isHorizontal)
    {
        Size = size;
        Row = row;
        Col = col;
        IsHorizontal = isHorizontal;
    }

    public bool IsHit(int row, int col)
    {
        if (IsHorizontal)
        {
            return row == Row && col >= Col && col < Col + Size;
        }
        return col == Col && row >= Row && row < Row + Size;
    }

    public bool IsSunk(int[,] grid)
    {
        for (int i = 0; i < Size; i++)
        {
            int checkRow = IsHorizontal ? Row : Row + i;
            int checkCol = IsHorizontal ? Col + i : Col;
            if (grid[checkRow, checkCol] != 3) // If any part is not hit
                return false;
        }
        return true;
    }
} 