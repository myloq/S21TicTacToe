using System.Security.Cryptography;
using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Service;

public class GameService(IRepository repository) : IGameService
{
    private readonly GameStatusExtension _statusExtension = new();
    public Game CreateGame(Guid userId)
    {
        Game game = new Game(Guid.NewGuid(), userId, null, new GameField(new int[3,3]), GameStatus.CrossTurn);
        int botOrder = RandomNumberGenerator.GetInt32(1, 3);
        if(botOrder == 1) game = GetBestMove(game);
        
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
        if (!ValidateGame(incoming, current)) throw new GameValidationException("Cannot validate the game");

        var status = CheckForWinner(incoming);
        if (status != GameStatus.InGame)
        {
            incoming.ChangeStatus(status);
            repository.Save(incoming);
            return incoming;
        }
        
        incoming.ChangeStatus(_statusExtension.GetOpponentStatus(incoming.Status));
        
        Game botMove = GetBestMove(incoming);
        
        status = CheckForWinner(botMove);
        botMove.ChangeStatus(status == GameStatus.InGame ? botMove.Status : status);
        
        repository.Save(botMove);
        return botMove;
    }
    
    public Game GetBestMove(Game game)
    {
        int player = GetPlayer(game.Status);
        int bestScore = int.MinValue;
        int bestIndex = -1;
        int[,] board = game.Field.Board;

        for (int i = 0; i < 9; i++)
        {
            if (board[i / 3, i % 3] != 0) continue;

            board[i / 3, i % 3] = player;
            int score = MiniMax(board, _statusExtension.GetOpponentStatus(game.Status), player, 0);
            board[i / 3, i % 3] = 0;
            
            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        if (bestIndex == -1) return game;

        Game result = new Game(game.Id, game.Player1Id, game.Player2Id, new GameField(game.Field.Board),
            _statusExtension.GetOpponentStatus(game.Status));
        result.Field.Board[bestIndex/3, bestIndex% 3] = player;
        return result;
    }
    
    private int MiniMax(int[,] board, GameStatus gameStatus, int role, int depth)
    {
        var status = CheckForWinner(board);
        switch (status)
        {
            case GameStatus.CrossWin: return role == 1 ? 10 - depth : depth - 10;
            case GameStatus.Draw: return 0;
            case GameStatus.ZeroWin: return role == 2 ? 10 - depth : depth - 10;
        }

        int currentPlayer = GetPlayer(gameStatus);

        int bestScore = currentPlayer == role ? int.MinValue : int.MaxValue;

        for (int i = 0; i < 9; i++)
        {
            if (board[i / 3, i % 3] != 0) continue;

            board[i / 3, i % 3] = currentPlayer;

            var nextStatus = currentPlayer == 1
                ? GameStatus.ZeroTurn
                : GameStatus.CrossTurn;

            int score = MiniMax(board, nextStatus, role, depth + 1);

            board[i / 3, i % 3] = 0;

            if (currentPlayer == role)
            {
                if (score > bestScore) bestScore = score;
            }
            else
            {
                if (score < bestScore) bestScore = score;
            }
        }

        return bestScore;
    }

    private static int GetPlayer(GameStatus status) =>
        status == GameStatus.CrossTurn ? 1 : 2;

    public bool ValidateGame(Game game, Game lastState)
    {
        if (game.Id != lastState.Id) return false;
        if (game.Status != lastState.Status) return false;
        if (game.Status != GameStatus.CrossTurn && game.Status != GameStatus.ZeroTurn) return false;

        GameField newField = game.Field;
        GameField oldField = lastState.Field;

        for (var i = 0; i < newField.Board.GetLength(0); i++)
        {
            for (var j = 0; j < newField.Board.GetLength(1); j++)
            {
                if (newField.Board[i, j] != oldField.Board[i, j] && newField.Board[i, j] != GetPlayer(game.Status))
                    return false;
            }
        }

        return true;
    }

    public GameStatus CheckForWinner(Game game) => CheckForWinner(game.Field.Board);
    
    private static GameStatus CheckForWinner(int[,] board)
    {
        for (var i = 0; i < board.GetLength(0); i++)
        {
            if (board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2] && 
                board[i, 0] != 0) return board[i, 0] == 1 ? GameStatus.CrossWin : GameStatus.ZeroWin;
        }
        
        for (var i = 0; i < board.GetLength(0); i++)
        {
            if (board[0, i] == board[1, i] && board[1, i] == board[2, i] && 
                board[0, i] != 0) return board[0, i] == 1 ? GameStatus.CrossWin : GameStatus.ZeroWin;
        }
        
        if (((board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2]) ||
             (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])) && 
            board[1, 1] != 0) return board[1, 1] == 1 ? GameStatus.CrossWin : GameStatus.ZeroWin;
        foreach (int i in board)
        {
            if (i == 0) return GameStatus.InGame;
        }
        return GameStatus.Draw;
    }
}