namespace TicTacToe.Domain.Exceptions;

public sealed class InvalidMoveException(string ms) : Exception(ms);