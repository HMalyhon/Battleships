namespace Battleships;

using Models;

public class GameManager
{
    private readonly Dictionary<string, BattleshipGame> games = new();

    public string CreateNewGame()
    {
        var gameId = Guid.NewGuid().ToString();
        var game = new BattleshipGame();
        game.InitializeGame();
        games[gameId] = game;
        return gameId;
    }

    public bool GameExists(string gameId)
    {
        return games.ContainsKey(gameId);
    }

    public ShotResult ProcessShot(string gameId, string coordinate)
    {
        var game = games[gameId];
        return game.ProcessPlayerShot(coordinate);
    }

    public bool PlaceShip(string gameId, ShipPlacement placement)
    {
        var game = games[gameId];
        return game.PlacePlayerShip(placement);
    }

    public GameBoard GetGameBoard(string gameId)
    {
        var game = games[gameId];
        return game.GetGameBoard();
    }

    public bool IsSetupComplete(string gameId)
    {
        var game = games[gameId];
        return game.IsSetupComplete();
    }
} 