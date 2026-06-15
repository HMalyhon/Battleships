namespace Battleships;

using Battleships.Models;

public class BattleshipGame
{
    private const int GridSize = 10;
    private const int Empty = 0;
    private const int Ship = 1;
    private const int Miss = 2;
    private const int Hit = 3;

    private static readonly (string Name, int Size)[] FleetConfiguration =
    {
        ("Battleship", 5),
        ("Destroyer", 4),
        ("Destroyer", 4)
    };

    private static readonly int TotalShipSquares = FleetConfiguration.Sum(s => s.Size);

    private readonly int[,] _playerGrid = new int[GridSize, GridSize];
    private readonly int[,] _computerGrid = new int[GridSize, GridSize];
    private readonly List<Ship> _playerShips = new();
    private readonly List<Ship> _computerShips = new();
    private readonly Random _random = new();
    private readonly ComputerAi _ai;

    private int _playerShipSquares;
    private int _playerHits;
    private int _computerHits;
    private bool _isInitialized;

    public BattleshipGame()
    {
        _ai = new ComputerAi(_random);
    }

    public void InitializeGame()
    {
        ClearGrid(_playerGrid);
        ClearGrid(_computerGrid);
        _playerShips.Clear();
        _computerShips.Clear();
        _playerShipSquares = 0;
        _playerHits = 0;
        _computerHits = 0;

        foreach (var (_, size) in FleetConfiguration)
            PlaceComputerShip(size);

        _isInitialized = true;
    }

    public bool PlacePlayerShip(ShipPlacement placement)
    {
        if (!TryParseCoordinates(placement.StartCoordinate, out int row, out int col))
            return false;

        string shipType = placement.ShipId.Split('-')[0];
        int size = shipType.Equals("battleship", StringComparison.OrdinalIgnoreCase) ? 5 : 4;

        if (!CanPlaceShip(_playerGrid, row, col, size, placement.IsHorizontal))
            return false;

        _playerShips.Add(new Ship(size, row, col, placement.IsHorizontal));
        WriteShipToGrid(_playerGrid, row, col, size, placement.IsHorizontal);
        _playerShipSquares += size;
        return true;
    }

    public ShotResult ProcessPlayerShot(string coordinate)
    {
        if (!TryParseCoordinates(coordinate, out int row, out int col))
            return new ShotResult("Invalid coordinates", false, false, null);

        var result = ProcessShot(_computerGrid, _computerShips, row, col, ref _playerHits);
        if (result.IsHit && _playerHits == TotalShipSquares)
            result = result with { Message = "Congratulations! You've won!" };

        if (!IsGameOver())
            MakeComputerMove();

        return result;
    }

    public GameBoard GetGameBoard() =>
        new(
            PlayerBoard: ConvertGridToBoard(_playerGrid),
            OpponentBoard: ConvertGridToBoard(_computerGrid, hideShips: true),
            IsGameOver: IsGameOver(),
            Winner: GetWinner()
        );

    public bool IsSetupComplete() => _playerShipSquares == TotalShipSquares;

    private void MakeComputerMove()
    {
        var target = _ai.ChooseTarget();
        var result = ProcessShot(_playerGrid, _playerShips, target.row, target.col, ref _computerHits);
        _ai.RegisterResult(target, result.IsHit);
    }

    private void PlaceComputerShip(int size)
    {
        while (true)
        {
            int row = _random.Next(GridSize);
            int col = _random.Next(GridSize);
            bool isHorizontal = _random.Next(2) == 0;

            if (!CanPlaceShip(_computerGrid, row, col, size, isHorizontal))
                continue;

            _computerShips.Add(new Ship(size, row, col, isHorizontal));
            WriteShipToGrid(_computerGrid, row, col, size, isHorizontal);
            return;
        }
    }

    private static void ClearGrid(int[,] grid)
    {
        for (int i = 0; i < GridSize; i++)
            for (int j = 0; j < GridSize; j++)
                grid[i, j] = Empty;
    }

    private static bool CanPlaceShip(int[,] grid, int row, int col, int size, bool isHorizontal)
    {
        if (isHorizontal && col + size > GridSize) return false;
        if (!isHorizontal && row + size > GridSize) return false;

        for (int i = 0; i < size; i++)
        {
            int r = isHorizontal ? row : row + i;
            int c = isHorizontal ? col + i : col;
            if (grid[r, c] != Empty) return false;
        }
        return true;
    }

    private static void WriteShipToGrid(int[,] grid, int row, int col, int size, bool isHorizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int r = isHorizontal ? row : row + i;
            int c = isHorizontal ? col + i : col;
            grid[r, c] = Ship;
        }
    }

    private static ShotResult ProcessShot(int[,] grid, List<Ship> ships, int row, int col, ref int hits)
    {
        switch (grid[row, col])
        {
            case Ship:
                grid[row, col] = Hit;
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

            case Empty:
                grid[row, col] = Miss;
                return new ShotResult("Miss!", false, false, null);

            default:
                return new ShotResult("You've already fired at these coordinates!", false, false, null);
        }
    }

    private string[][] ConvertGridToBoard(int[,] grid, bool hideShips = false)
    {
        var board = new string[GridSize][];
        for (int i = 0; i < GridSize; i++)
        {
            board[i] = new string[GridSize];
            for (int j = 0; j < GridSize; j++)
            {
                board[i][j] = grid[i, j] switch
                {
                    Empty => "·",
                    Ship => hideShips ? "·" : "S",
                    Miss => "O",
                    Hit => "X",
                    _ => "·"
                };
            }
        }
        return board;
    }

    private bool IsGameOver() =>
        _isInitialized && IsSetupComplete() &&
        (_playerHits == TotalShipSquares || _computerHits == _playerShipSquares);

    private string? GetWinner()
    {
        if (!IsGameOver()) return null;
        return _playerHits == TotalShipSquares ? "Player" : "Computer";
    }

    private static bool TryParseCoordinates(string input, out int row, out int col)
    {
        row = col = 0;
        if (string.IsNullOrEmpty(input) || input.Length < 2) return false;

        input = input.ToUpper();
        col = input[0] - 'A';
        if (!int.TryParse(input[1..], out row)) return false;

        return col >= 0 && col < GridSize && row >= 0 && row < GridSize;
    }
}
