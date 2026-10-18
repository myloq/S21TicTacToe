namespace TicTacToe.Domain.Exceptions;

public sealed class GameValidationException(string ms) : Exception(ms);