import React from 'react';
import { Paper, Typography } from '@mui/material';
import { styled } from '@mui/material/styles';

const StatusPaper = styled(Paper)(({ theme }) => ({
  padding: theme.spacing(2),
  marginBottom: theme.spacing(2)
}));

interface GameStatusProps {
  message: string;
}

const GameStatus: React.FC<GameStatusProps> = ({ message }) => {
  return (
    <StatusPaper elevation={2}>
      <Typography variant="h6">{message}</Typography>
    </StatusPaper>
  );
};

export default GameStatus;
