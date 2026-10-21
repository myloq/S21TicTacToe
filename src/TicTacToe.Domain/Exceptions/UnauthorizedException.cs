namespace TicTacToe.Domain.Exceptions;

public sealed class UnauthorizedException(string message) : Exception(message);