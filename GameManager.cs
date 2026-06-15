namespace Battleships;

using System.Collections.Concurrent;
using Models;

public class GameManager
{
    private readonly ConcurrentDictionary<string, BattleshipGame> _games = new();
    private readonly ConcurrentDictionary<string, object> _locks = new();

    public string CreateNewGame()
    {
        var gameId = Guid.NewGuid().ToString();
        var game = new BattleshipGame();
        game.InitializeGame();
        _games[gameId] = game;
        _locks[gameId] = new object();
        return gameId;
    }

    public bool GameExists(string gameId) => _games.ContainsKey(gameId);

    public ShotResult ProcessShot(string gameId, string coordinate)
    {
        lock (GetLock(gameId))
            return _games[gameId].ProcessPlayerShot(coordinate);
    }

    public bool PlaceShip(string gameId, ShipPlacement placement)
    {
        lock (GetLock(gameId))
            return _games[gameId].PlacePlayerShip(placement);
    }

    public GameBoard GetGameBoard(string gameId)
    {
        lock (GetLock(gameId))
            return _games[gameId].GetGameBoard();
    }

    public bool IsSetupComplete(string gameId)
    {
        lock (GetLock(gameId))
            return _games[gameId].IsSetupComplete();
    }

    private object GetLock(string gameId) => _locks[gameId];
}