import React, { useState, useEffect, useRef } from 'react';
import { Container, Button, Box, Typography, Paper, Grid } from '@mui/material';
import GameBoard from './components/GameBoard';
import GameStatus from './components/GameStatus';
import ShipPlacement from './components/ShipPlacement';
import { gameService } from './services/api';
import { Ship, GameBoard as GameBoardType } from './types';

function App() {
  const [gameId, setGameId] = useState<string | null>(null);
  const [playerBoard, setPlayerBoard] = useState<string[][]>([]);
  const [opponentBoard, setOpponentBoard] = useState<string[][]>([]);
  const [message, setMessage] = useState<string>('Welcome to Battleships!');
  const [isGameOver, setIsGameOver] = useState(false);
  const [winner, setWinner] = useState<string | null>(null);
  const [isSetupPhase, setIsSetupPhase] = useState(true);
  const [availableShips, setAvailableShips] = useState<Ship[]>([
    { type: 'battleship', size: 5, placed: false, id: 'battleship-1' },
    { type: 'destroyer', size: 4, placed: false, id: 'destroyer-1' },
    { type: 'destroyer', size: 4, placed: false, id: 'destroyer-2' }
  ]);

  const initialized = useRef(false);

  const startNewGame = async () => {
    try {
      const newGameId = await gameService.createNewGame();
      setGameId(newGameId);
      setMessage('Place your ships!');
      setIsGameOver(false);
      setWinner(null);
      setIsSetupPhase(true);
      setAvailableShips([
        { type: 'battleship', size: 5, placed: false, id: 'battleship-1' },
        { type: 'destroyer', size: 4, placed: false, id: 'destroyer-1' },
        { type: 'destroyer', size: 4, placed: false, id: 'destroyer-2' }
      ]);
      await updateBoard(newGameId);
    } catch (error) {
      setMessage('Failed to start new game. Please try again.');
    }
  };

  const updateBoard = async (currentGameId: string) => {
    try {
      const gameBoard = await gameService.getBoard(currentGameId);
      setPlayerBoard(gameBoard.playerBoard);
      setOpponentBoard(gameBoard.opponentBoard);
      setIsGameOver(gameBoard.isGameOver);
      setWinner(gameBoard.winner);

      if (gameBoard.isGameOver && gameBoard.winner) {
        setMessage(`Game Over! ${gameBoard.winner} wins!`);
      }
    } catch (error) {
      setMessage('Failed to update game board.');
    }
  };

  const handlePlaceShip = async (shipId: string, coordinate: string, isHorizontal: boolean) => {
    if (!gameId) return;

    try {
      const success = await gameService.placeShip(gameId, {
        shipId,
        startCoordinate: coordinate,
        isHorizontal
      });

      if (success) {
        await updateBoard(gameId);

        setAvailableShips(prevShips => {
          const updatedShips = prevShips.map(ship =>
            ship.id === shipId && !ship.placed ? { ...ship, placed: true } : ship
          );

          const remainingShips = updatedShips.filter(ship => !ship.placed).length;

          if (remainingShips === 0) {
            setIsSetupPhase(false);
            setMessage('All ships placed! Start firing at the opponent!');
          } else {
            setMessage(`Ship placed! ${remainingShips} ships remaining.`);
          }

          return updatedShips;
        });
      } else {
        setMessage('Invalid ship placement! Try again.');
      }
    } catch (error) {
      setMessage('Failed to place ship. Please try again.');
    }
  };

  const handleCellClick = async (row: number, col: number) => {
    if (!gameId || isGameOver || isSetupPhase) return;

    try {
      const coordinate = `${String.fromCharCode(65 + col)}${row}`;
      const result = await gameService.makeShot(gameId, coordinate);
      setMessage(result.message);

      if (result.isSunk) {
        setMessage(`${result.message} You've sunk a ${result.shipType}!`);
      }

      await updateBoard(gameId);
    } catch (error) {
      setMessage('Failed to make shot. Please try again.');
    }
  };

  useEffect(() => {
    if (!initialized.current) {
      initialized.current = true;
      startNewGame();
    }
  }, []);

  return (
    <Container maxWidth="lg">
      <Box sx={{ my: 4, textAlign: 'center' }}>
        <Typography variant="h3" component="h1" gutterBottom>
          Battleships
        </Typography>
        <GameStatus message={message} />
        <Paper sx={{ p: 3, mb: 3 }}>
          {isSetupPhase ? (
            <ShipPlacement
              board={playerBoard}
              availableShips={availableShips}
              onPlaceShip={handlePlaceShip}
            />
          ) : (
            <Grid container spacing={4}>
              <Grid item xs={12} md={6}>
                <Typography variant="h6" gutterBottom>
                  Your Board
                </Typography>
                <GameBoard board={playerBoard} onCellClick={() => {}} />
              </Grid>
              <Grid item xs={12} md={6}>
                <Typography variant="h6" gutterBottom>
                  Opponent's Board
                </Typography>
                <GameBoard board={opponentBoard} onCellClick={handleCellClick} />
              </Grid>
            </Grid>
          )}
          <Box sx={{ mt: 3 }}>
            <Button
              variant="contained"
              size="large"
              onClick={startNewGame}
              disabled={!isGameOver || (!isSetupPhase && availableShips.some(ship => !ship.placed))}
            >
              New Game
            </Button>
          </Box>
        </Paper>
      </Box>
    </Container>
  );
}

export default App;
