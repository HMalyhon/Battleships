import React from 'react';
import { Box, Button, Typography, ToggleButton, ToggleButtonGroup } from '@mui/material';
import { Ship } from '../types';
import GameBoard from './GameBoard';

interface ShipPlacementProps {
  board: string[][];
  availableShips: Ship[];
  onPlaceShip: (shipId: string, coordinate: string, isHorizontal: boolean) => void;
}

const ShipPlacement: React.FC<ShipPlacementProps> = ({ board, availableShips, onPlaceShip }) => {
  const [selectedShip, setSelectedShip] = React.useState<Ship | null>(null);
  const [orientation, setOrientation] = React.useState<boolean>(true);

  const handleCellClick = (row: number, col: number) => {
    if (selectedShip) {
      const coordinate = `${String.fromCharCode(65 + col)}${row}`;
      onPlaceShip(selectedShip.id, coordinate, orientation);
    }
  };

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Place Your Ships
      </Typography>
      <Box sx={{ mb: 2 }}>
        <ToggleButtonGroup
          value={orientation}
          exclusive
          onChange={(_, value) => setOrientation(value)}
          aria-label="ship orientation"
        >
          <ToggleButton value={true}>Horizontal</ToggleButton>
          <ToggleButton value={false}>Vertical</ToggleButton>
        </ToggleButtonGroup>
      </Box>
      <Box sx={{ mb: 2 }}>
        {availableShips.map(ship => (
          <Button
            key={ship.id}
            variant={selectedShip?.id === ship.id ? 'contained' : 'outlined'}
            disabled={ship.placed}
            onClick={() => setSelectedShip(ship)}
            sx={{ mr: 1 }}
          >
            {ship.type} ({ship.size})
          </Button>
        ))}
      </Box>
      <GameBoard board={board} onCellClick={handleCellClick} />
    </Box>
  );
};

export default ShipPlacement;
