using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Interface;

public interface IUserService
{
    bool Register(SignUpRequest signUpRequest);

    Guid? Authenticate(SignUpRequest signUpRequest);

    User? GetById(Guid id);
}