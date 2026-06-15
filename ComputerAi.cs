namespace Battleships;

public class ComputerAi
{
    private const int GridSize = 10;

    private readonly Random _random;
    private readonly List<(int row, int col)> _hits = new();
    private readonly List<(int row, int col)> _potentialTargets = new();
    private readonly HashSet<(int row, int col)> _shotCells = new();

    public ComputerAi(Random random)
    {
        _random = random;
    }

    public (int row, int col) ChooseTarget()
    {
        var target = _potentialTargets.Count > 0 ? PopNextPotentialTarget() : PickRandomTarget();
        _shotCells.Add(target);
        return target;
    }

    public void RegisterResult((int row, int col) target, bool isHit)
    {
        if (!isHit) return;

        _hits.Add(target);
        AddAdjacentTargets(target);
    }

    private (int row, int col) PopNextPotentialTarget()
    {
        var target = _potentialTargets[0];
        _potentialTargets.RemoveAt(0);
        return target;
    }

    private (int row, int col) PickRandomTarget()
    {
        while (true)
        {
            var candidate = (row: _random.Next(GridSize), col: _random.Next(GridSize));
            if (!_shotCells.Contains(candidate))
                return candidate;
        }
    }

    private void AddAdjacentTargets((int row, int col) cell)
    {
        var neighbours = new (int row, int col)[]
        {
            (cell.row - 1, cell.col),
            (cell.row + 1, cell.col),
            (cell.row, cell.col - 1),
            (cell.row, cell.col + 1)
        };

        foreach (var n in neighbours)
        {
            if (IsValidCandidate(n))
                _potentialTargets.Add(n);
        }

        if (_hits.Count >= 2)
            PrioritizeTargetsInLine();
    }

    private bool IsValidCandidate((int row, int col) cell)
    {
        return cell.row >= 0 && cell.row < GridSize &&
               cell.col >= 0 && cell.col < GridSize &&
               !_shotCells.Contains(cell) &&
               !_potentialTargets.Contains(cell);
    }

    private void PrioritizeTargetsInLine()
    {
        var orderedHits = _hits.OrderBy(h => h.row).ThenBy(h => h.col).ToList();
        bool isHorizontal = orderedHits.Take(2).Select(h => h.row).Distinct().Count() == 1;
        bool isVertical = orderedHits.Take(2).Select(h => h.col).Distinct().Count() == 1;

        if (!isHorizontal && !isVertical) return;

        _potentialTargets.Sort((a, b) =>
            IsInLineWithHits(b, isHorizontal).CompareTo(IsInLineWithHits(a, isHorizontal)));
    }

    private int IsInLineWithHits((int row, int col) cell, bool isHorizontal) =>
        _hits.Any(h => isHorizontal ? h.row == cell.row : h.col == cell.col) ? 1 : 0;
}
