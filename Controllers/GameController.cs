namespace Battleships.Controllers;

using Microsoft.AspNetCore.Mvc;
using Battleships.Models;

[ApiController]
[Route("game")]
public class GameController : ControllerBase
{
    private readonly GameManager _gameManager;

    public GameController(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    [HttpGet("new")]
    [ProducesResponseType(typeof(GameIdResponse), StatusCodes.Status200OK)]
    public IActionResult CreateNewGame()
    {
        var gameId = _gameManager.CreateNewGame();
        return Ok(new GameIdResponse(gameId));
    }

    [HttpPost("{gameId}/place-ship")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult PlaceShip(string gameId, [FromBody] ShipPlacement placement)
    {
        if (!_gameManager.GameExists(gameId))
            return NotFound("Game not found");

        if (_gameManager.IsSetupComplete(gameId))
            return BadRequest("All ships are already placed");

        var success = _gameManager.PlaceShip(gameId, placement);
        if (!success)
            return BadRequest("Invalid ship placement");

        return Ok(success);
    }

    [HttpPost("{gameId}/shoot")]
    [ProducesResponseType(typeof(ShotResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult MakeShot(string gameId, [FromBody] ShotRequest shot)
    {
        if (!_gameManager.GameExists(gameId))
            return NotFound("Game not found");

        if (!_gameManager.IsSetupComplete(gameId))
            return BadRequest("Cannot shoot until all ships are placed");

        var result = _gameManager.ProcessShot(gameId, shot.Coordinate);
        return Ok(result);
    }

    [HttpGet("{gameId}/board")]
    [ProducesResponseType(typeof(GameBoard), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetBoard(string gameId)
    {
        if (!_gameManager.GameExists(gameId))
            return NotFound("Game not found");

        var board = _gameManager.GetGameBoard(gameId);
        return Ok(board);
    }
}