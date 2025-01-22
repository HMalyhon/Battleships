import axios from 'axios';
import { GameBoard, ShotResult, GameIdResponse, ShipPlacement } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL;

export const gameService = {
  createNewGame: async (): Promise<string> => {
    const response = await axios.get<GameIdResponse>(`${API_BASE_URL}/game/new`);
    return response.data.gameId;
  },

  placeShip: async (gameId: string, placement: ShipPlacement): Promise<boolean> => {
    const response = await axios.post<boolean>(
      `${API_BASE_URL}/game/${gameId}/place-ship`,
      placement
    );
    return response.data;
  },

  makeShot: async (gameId: string, coordinate: string): Promise<ShotResult> => {
    const response = await axios.post<ShotResult>(`${API_BASE_URL}/game/${gameId}/shoot`, {
      coordinate
    });
    return response.data;
  },

  getBoard: async (gameId: string): Promise<GameBoard> => {
    const response = await axios.get<GameBoard>(`${API_BASE_URL}/game/${gameId}/board`);
    return response.data;
  }
};
