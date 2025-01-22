namespace Battleships.Models;

public record GameBoard(
    string[][] PlayerBoard,
    string[][] OpponentBoard,
    bool IsGameOver,
    string? Winner
); 