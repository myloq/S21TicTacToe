using System.Security.Cryptography;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Service;

public class GameService(IRepository repository) : IGameService
{
    public Game CreateGame(Guid userId, bool againstBot)
    {
        int player1Symbol = RandomNumberGenerator.GetInt32(1, 3);
        int player2Symbol = player1Symbol == 1 ? 2 : 1;
        Guid? currentPlayerId = player1Symbol == 1 ? userId : null;
        Game game = new Game(Guid.NewGuid(), userId, null, new GameField(new int[3, 3]), GameStatus.WaitingForPlayers,
            currentPlayerId, null, player1Symbol, player2Symbol);

        if (againstBot) // Игра с ботом
        {
            if (currentPlayerId == null) game = GetBestMove(game);
            game.ChangeStatus(GameStatus.InProgress);
        }
        // Иначе "WaitingForPlayers" - игра с игроком

        repository.Save(game);
        return game;
    }

    public Game GetGame(Guid userId, Guid gameId)
    {
        var game = repository.GetGame(gameId) ?? throw new GameNotFoundException(gameId);
        if (game.Player1Id != userId && game.Player2Id != userId) throw new GameNotFoundException(gameId);
        return game;
    }

    public Game MakeMove(Guid userId, Guid gameId, Game incoming)
    {
        var current = repository.GetGame(gameId) ?? throw new GameNotFoundException(gameId);
        if (current.Player1Id != userId && current.Player2Id != userId) throw new GameNotFoundException(gameId);
        if (userId != current.CurrentPlayerId) throw new InvalidMoveException("Current player Id is not valid");
        if (!ValidateGame(incoming, current)) throw new GameValidationException("Cannot validate the game");

        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
            current.Field.Board[i, j] = incoming.Field.Board[i, j];

        var status = CheckForWinner(current);
        if (status is GameStatus.Win or GameStatus.Draw)
        {
            current.ChangeStatus(status);
            if (status == GameStatus.Win) current.WinnerId = userId;
            repository.Save(current);
            return current;
        }

        current.ChangeCurrentPlayerId();

        if (current.Player2Id == null)
        {
            var botMove = GetBestMove(current);
            status = CheckForWinner(botMove);

            if (status is GameStatus.Win or GameStatus.Draw)
            {
                botMove.ChangeStatus(status);
                if (status == GameStatus.Win) botMove.WinnerId = null;
            }

            repository.Save(botMove);
            return botMove;
        }

        repository.Save(current);
        return current;
    }

    public Game GetBestMove(Game game)
    {
        int botSymbol = game.Player2Symbol;
        int bestScore = int.MinValue;
        int bestRow = -1, bestCol = -1;
        int[,] board = (int[,])game.Field.Board.Clone();

        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            if (board[i, j] == 0)
            {
                board[i, j] = botSymbol;
                int nextSymbol = botSymbol == 1 ? 2 : 1;
                int score = MiniMax(board, nextSymbol, botSymbol, 0);
                board[i, j] = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestRow = i;
                    bestCol = j;
                }
            }
        }

        if (bestRow == -1) return game;

        Game result = new Game(game.Id, game.Player1Id, game.Player2Id, new GameField(board),
            game.Status, game.CurrentPlayerId, null, game.Player1Symbol, game.Player2Symbol);

        result.Field.Board[bestRow, bestCol] = botSymbol;
        result.ChangeCurrentPlayerId();
        return result;
    }

    private int MiniMax(int[,] board, int currentSymbol, int botSymbol, int depth)
    {
        var status = CheckForWinner(board);
        if (status is GameStatus.Win or GameStatus.Draw)
        {
            if (status == GameStatus.Draw) return 0;
            if (currentSymbol != botSymbol) return 10 - depth;
            return depth - 10;
        }

        bool isMaximizing = currentSymbol == botSymbol;
        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;
        int nextSymbol = currentSymbol == 1 ? 2 : 1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == 0)
                {
                    board[i, j] = currentSymbol;
                    int score = MiniMax(board, nextSymbol, botSymbol, depth + 1);
                    board[i, j] = 0;

                    if (isMaximizing) bestScore = Math.Max(bestScore, score);
                    else bestScore = Math.Min(bestScore, score);
                }
            }
        }

        return bestScore;
    }

    public bool ValidateGame(Game incoming, Game current)
    {
        if (incoming.Id != current.Id) return false;
        if (incoming.Status != current.Status) return false;
        if (current.Status != GameStatus.InProgress) return false;

        int expectedSymbol = current.GetPlayerSymbol(current.CurrentPlayerId);
        int changesCount = 0;

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (incoming.Field.Board[i, j] != current.Field.Board[i, j])
                {
                    changesCount++;
                    if (current.Field.Board[i, j] != 0 || incoming.Field.Board[i, j] != expectedSymbol)
                        return false;
                }
            }
        }

        return changesCount == 1;
    }

    public GameStatus CheckForWinner(Game game) => CheckForWinner(game.Field.Board);

    private static GameStatus CheckForWinner(int[,] board)
    {
        for (var i = 0; i < board.GetLength(0); i++)
            if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && board[i, 0] != 0)
                return GameStatus.Win;

        for (var i = 0; i < board.GetLength(0); i++)
            if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && board[0, i] != 0)
                return GameStatus.Win;

        if (((board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2]) ||
             (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])) &&
            board[1, 1] != 0) return GameStatus.Win;
        ;

        foreach (int i in board)
            if (i == 0)
                return GameStatus.InProgress;

        return GameStatus.Draw;
    }

    public IEnumerable<Game> GetAvailableGames(Guid userId)
    {
        var games = repository.GetAvailableGames();

        return games.Where(g => g.Player1Id != userId);
    }

    public Game JoinGame(Guid currentUserId, Guid gameId)
    {
        var game = repository.GetGame(gameId);
        if (game is null || game.Status != GameStatus.WaitingForPlayers) throw new GameNotFoundException(gameId);

        if (game.Player1Id == currentUserId)
            throw new GameValidationException("You cannot join your own game");

        game.ChangeStatus(GameStatus.InProgress);
        game.Player2Id = currentUserId;

        if (game.CurrentPlayerId == null)
        {
            game.SetCurrentPlayerId(currentUserId);
        }

        repository.Save(game);
        return game;
    }
}