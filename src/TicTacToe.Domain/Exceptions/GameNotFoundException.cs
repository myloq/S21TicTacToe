namespace TicTacToe.Domain.Exceptions;

public sealed class GameNotFoundException(Guid id)
    : Exception($"Game with id {id} was not found.");