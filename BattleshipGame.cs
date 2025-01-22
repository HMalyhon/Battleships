namespace Battleships;

using Battleships.Models;

public class BattleshipGame
{
    private readonly int[,] playerGrid = new int[10, 10];
    private readonly int[,] computerGrid = new int[10, 10];
    private readonly List<Ship> playerShips = new();
    private readonly List<Ship> computerShips = new();
    private int playerShipSquares = 0;
    private int computerShipSquares = 0;
    private int playerHits = 0;
    private int computerHits = 0;
    private readonly Random random = new();
    private bool isInitialized = false;
    private List<(int row, int col)> computerHitsList = new();
    private List<(int row, int col)> potentialTargets = new();

    public void InitializeGame()
    {
        // Reset all fields
        ClearGrid(playerGrid);
        ClearGrid(computerGrid);
        playerShips.Clear();
        computerShips.Clear();
        playerShipSquares = 0;
        computerShipSquares = 0;
        playerHits = 0;
        computerHits = 0;
        computerHitsList.Clear();
        potentialTargets.Clear();

        // Place computer ships
        PlaceComputerShip(5); // Battleship
        PlaceComputerShip(4); // Destroyer 1
        PlaceComputerShip(4); // Destroyer 2
        computerShipSquares = 13;
        isInitialized = true;
    }

    private void ClearGrid(int[,] grid)
    {
        for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                grid[i, j] = 0;
    }

    public bool PlacePlayerShip(ShipPlacement placement)
    {
        if (!TryParseCoordinates(placement.StartCoordinate, out int row, out int col))
            return false;

        // Extract ship type from ID (e.g., "battleship-1" -> "battleship")
        string shipType = placement.ShipId.Split('-')[0];
        int size = shipType.ToLower() == "battleship" ? 5 : 4;

        if (!CanPlaceShip(playerGrid, row, col, size, placement.IsHorizontal))
            return false;

        playerShips.Add(new Ship(size, row, col, placement.IsHorizontal));
        for (int i = 0; i < size; i++)
        {
            if (placement.IsHorizontal)
                playerGrid[row, col + i] = 1;
            else
                playerGrid[row + i, col] = 1;
        }
        playerShipSquares += size;
        return true;
    }

    private void PlaceComputerShip(int size)
    {
        bool placed = false;
        while (!placed)
        {
            int row = random.Next(10);
            int col = random.Next(10);
            bool isHorizontal = random.Next(2) == 0;

            if (CanPlaceShip(computerGrid, row, col, size, isHorizontal))
            {
                computerShips.Add(new Ship(size, row, col, isHorizontal));
                for (int i = 0; i < size; i++)
                {
                    if (isHorizontal)
                        computerGrid[row, col + i] = 1;
                    else
                        computerGrid[row + i, col] = 1;
                }
                placed = true;
            }
        }
    }

    private bool CanPlaceShip(int[,] grid, int row, int col, int size, bool isHorizontal)
    {
        if (isHorizontal && col + size > 10) return false;
        if (!isHorizontal && row + size > 10) return false;

        for (int i = 0; i < size; i++)
        {
            if (isHorizontal)
            {
                if (grid[row, col + i] != 0) return false;
            }
            else
            {
                if (grid[row + i, col] != 0) return false;
            }
        }
        return true;
    }

    public ShotResult ProcessPlayerShot(string coordinate)
    {
        if (!TryParseCoordinates(coordinate, out int row, out int col))
            return new ShotResult("Invalid coordinates", false, false, null);

        var result = ProcessShot(computerGrid, computerShips, row, col, ref playerHits);
        if (result.IsHit && playerHits == computerShipSquares)
            result = result with { Message = "Congratulations! You've won!" };

        // Computer's turn
        if (!IsGameOver())
        {
            MakeComputerMove();
        }

        return result;
    }

    private void MakeComputerMove()
    {
        bool validShot = false;
        while (!validShot)
        {
            (int row, int col) target;

            if (potentialTargets.Count > 0)
            {
                // Try adjacent cells to previous hits
                target = GetNextTarget();
            }
            else
            {
                // Random shot if no hits to follow up
                target = GetRandomTarget();
            }

            var result = ProcessShot(playerGrid, playerShips, target.row, target.col, ref computerHits);
            validShot = true;

            if (result.IsHit)
            {
                computerHitsList.Add(target);
                AddAdjacentTargets(target.row, target.col);
            }
            else
            {
                // Remove missed target from potential targets
                potentialTargets.Remove(target);
            }
        }
    }

    private (int row, int col) GetNextTarget()
    {
        // Get the next potential target
        var target = potentialTargets[0];
        potentialTargets.RemoveAt(0);
        return target;
    }

    private (int row, int col) GetRandomTarget()
    {
        while (true)
        {
            int row = random.Next(10);
            int col = random.Next(10);
            
            if (playerGrid[row, col] != 2 && playerGrid[row, col] != 3)
            {
                return (row, col);
            }
        }
    }

    private void AddAdjacentTargets(int row, int col)
    {
        // Add all adjacent cells that haven't been shot at yet
        var adjacentCells = new List<(int row, int col)>
        {
            (row - 1, col), // Up
            (row + 1, col), // Down
            (row, col - 1), // Left
            (row, col + 1)  // Right
        };

        foreach (var cell in adjacentCells)
        {
            if (IsValidTarget(cell.row, cell.col))
            {
                potentialTargets.Add(cell);
            }
        }

        // Prioritize targets in line with multiple hits
        if (computerHitsList.Count >= 2)
        {
            PrioritizeTargetsInLine();
        }
    }

    private void PrioritizeTargetsInLine()
    {
        // Check if we have multiple hits in a line
        var orderedHits = computerHitsList.OrderBy(h => h.row).ThenBy(h => h.col).ToList();
        bool isHorizontal = orderedHits.Count >= 2 && 
            orderedHits.Take(2).Select(h => h.row).Distinct().Count() == 1;
        bool isVertical = orderedHits.Count >= 2 && 
            orderedHits.Take(2).Select(h => h.col).Distinct().Count() == 1;

        if (isHorizontal || isVertical)
        {
            // Reorder potential targets to prioritize those in line with the hits
            potentialTargets = potentialTargets
                .OrderByDescending(t => IsInLineWithHits(t.row, t.col, isHorizontal))
                .ToList();
        }
    }

    private bool IsInLineWithHits(int row, int col, bool isHorizontal)
    {
        return computerHitsList.Any(h => 
            isHorizontal ? h.row == row : h.col == col);
    }

    private bool IsValidTarget(int row, int col)
    {
        return row >= 0 && row < 10 && 
               col >= 0 && col < 10 && 
               playerGrid[row, col] != 2 && 
               playerGrid[row, col] != 3 &&
               !potentialTargets.Contains((row, col));
    }

    private ShotResult ProcessShot(int[,] grid, List<Ship> ships, int row, int col, ref int hits)
    {
        if (grid[row, col] == 1) // Hit
        {
            grid[row, col] = 3;
            hits++;

            foreach (var ship in ships)
            {
                if (ship.IsHit(row, col) && ship.IsSunk(grid))
                {
                    var shipType = ship.Size == 5 ? "Battleship" : "Destroyer";
                    return new ShotResult($"Hit! You've sunk a {shipType}!", true, true, shipType);
                }
            }
            return new ShotResult("Hit!", true, false, null);
        }
        else if (grid[row, col] == 0)
        {
            grid[row, col] = 2;
            return new ShotResult("Miss!", false, false, null);
        }
        else
        {
            return new ShotResult("You've already fired at these coordinates!", false, false, null);
        }
    }

    public GameBoard GetGameBoard()
    {
        return new GameBoard(
            PlayerBoard: ConvertGridToBoard(playerGrid),
            OpponentBoard: ConvertGridToBoard(computerGrid, hideShips: true),
            IsGameOver: IsGameOver(),
            Winner: GetWinner()
        );
    }

    private string[][] ConvertGridToBoard(int[,] grid, bool hideShips = false)
    {
        var board = new string[10][];
        for (int i = 0; i < 10; i++)
        {
            board[i] = new string[10];
            for (int j = 0; j < 10; j++)
            {
                board[i][j] = grid[i, j] switch
                {
                    0 => "·",
                    1 => hideShips ? "·" : "S",
                    2 => "O",
                    3 => "X",
                    _ => "·"
                };
            }
        }
        return board;
    }

    private bool IsGameOver() =>
        isInitialized && IsSetupComplete() && 
        (playerHits == computerShipSquares || computerHits == playerShipSquares);

    private string? GetWinner()
    {
        if (!IsGameOver()) return null;
        return playerHits == computerShipSquares ? "Player" : "Computer";
    }

    private bool TryParseCoordinates(string input, out int row, out int col)
    {
        row = col = 0;
        input = input.ToUpper();
        if (input.Length < 2) return false;

        col = input[0] - 'A';
        if (!int.TryParse(input[1..], out row)) return false;

        return col >= 0 && col < 10 && row >= 0 && row < 10;
    }

    public bool IsSetupComplete() =>
        playerShipSquares == 13; // 5 + 4 + 4
} 