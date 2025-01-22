namespace Battleships.Models;

public record ShotResult(string Message, bool IsHit, bool IsSunk, string? ShipType); 