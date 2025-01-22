export interface GameBoard {
  playerBoard: string[][];
  opponentBoard: string[][];
  isGameOver: boolean;
  winner: string | null;
}

export interface ShotResult {
  message: string;
  isHit: boolean;
  isSunk: boolean;
  shipType: string | null;
}

export interface GameIdResponse {
  gameId: string;
}

export interface ShipPlacement {
  shipId: string;
  startCoordinate: string;
  isHorizontal: boolean;
}

export interface Ship {
  type: 'battleship' | 'destroyer';
  size: number;
  placed: boolean;
  id: string;
}
