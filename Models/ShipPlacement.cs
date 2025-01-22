namespace Battleships.Models;

public record ShipPlacement(
    string ShipId,
    string StartCoordinate,
    bool IsHorizontal
);