# Battleships

A classic Battleships game with a .NET 8 Web API backend and a React + TypeScript frontend.

You place a fleet on a 10x10 grid, then trade shots with a computer opponent that uses simple targeting heuristics (random until a hit, then probes neighbours and lines up subsequent hits).

## Stack

- **Backend**: ASP.NET Core 8 Web API, Swashbuckle (Swagger UI)
- **Frontend**: React 18, TypeScript, Material UI, Axios

## Project layout

```
.
├── BattleshipGame.cs        # core game state & rules
├── ComputerAi.cs            # opponent targeting logic
├── GameManager.cs           # in-memory game registry (thread-safe)
├── Controllers/             # HTTP endpoints
├── Models/                  # request/response records
└── client/                  # React frontend
```

## Running locally

### Backend

```bash
dotnet run
```

The API starts on `http://localhost:5000` (and `https://localhost:5001`).
Swagger UI is at `http://localhost:5000/swagger`.

### Frontend

```bash
cd client
cp .env.example .env
npm install
npm start
```

Opens on `http://localhost:3000` and talks to the backend at the URL from `.env`.

## API

| Method | Route                          | Purpose                            |
|--------|--------------------------------|------------------------------------|
| GET    | `/game/new`                    | Create a new game, returns gameId  |
| POST   | `/game/{gameId}/place-ship`    | Place one of the player's ships    |
| POST   | `/game/{gameId}/shoot`         | Fire at a coordinate (e.g. `B4`)   |
| GET    | `/game/{gameId}/board`         | Get current board state            |

## Fleet

| Ship       | Size |
|------------|------|
| Battleship | 5    |
| Destroyer  | 4    |
| Destroyer  | 4    |

## License

MIT — see [LICENSE](LICENSE).
