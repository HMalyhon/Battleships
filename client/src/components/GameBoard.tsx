import React from 'react';
import { Grid, Paper, Typography, Box } from '@mui/material';
import { styled } from '@mui/material/styles';

interface GameBoardProps {
  board: string[][];
  onCellClick: (row: number, col: number) => void;
}

const Cell = styled(Paper)(({ theme }) => ({
  height: 40,
  width: 40,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  cursor: 'pointer',
  fontSize: '1.2rem',
  fontWeight: 'bold',
  backgroundColor: theme.palette.grey[100],
  '&:hover': {
    backgroundColor: theme.palette.grey[200]
  },
  '&.hit': {
    backgroundColor: theme.palette.error.light,
    color: theme.palette.error.contrastText
  },
  '&.miss': {
    backgroundColor: theme.palette.grey[300],
    color: theme.palette.text.secondary
  }
}));

const HeaderCell = styled(Box)(({ theme }) => ({
  height: 40,
  width: 40,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  fontWeight: 'bold',
  color: theme.palette.text.primary
}));

const GameBoard: React.FC<GameBoardProps> = ({ board, onCellClick }) => {
  const columns = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

  const getCellClass = (cell: string) => {
    switch (cell) {
      case 'X':
        return 'hit';
      case 'O':
        return 'miss';
      default:
        return '';
    }
  };

  return (
    <Box sx={{ mt: 3, display: 'flex', justifyContent: 'center' }}>
      <Grid container spacing={0.5} sx={{ width: 'auto' }}>
        {/* Column Headers */}
        <Grid item xs={12}>
          <Grid container spacing={0.5}>
            <Grid item>
              <HeaderCell /> {/* Empty corner cell */}
            </Grid>
            {columns.map(col => (
              <Grid item key={col}>
                <HeaderCell>
                  <Typography variant="h6">{col}</Typography>
                </HeaderCell>
              </Grid>
            ))}
          </Grid>
        </Grid>

        {/* Game Grid */}
        {board.map((row, rowIndex) => (
          <Grid item xs={12} key={rowIndex}>
            <Grid container spacing={0.5}>
              {/* Row Header */}
              <Grid item>
                <HeaderCell>
                  <Typography variant="h6">{rowIndex}</Typography>
                </HeaderCell>
              </Grid>
              {/* Row Cells */}
              {row.map((cell, colIndex) => (
                <Grid item key={`${rowIndex}-${colIndex}`}>
                  <Cell
                    className={getCellClass(cell)}
                    onClick={() => onCellClick(rowIndex, colIndex)}
                    elevation={1}
                  >
                    {cell}
                  </Cell>
                </Grid>
              ))}
            </Grid>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
};

export default GameBoard;
