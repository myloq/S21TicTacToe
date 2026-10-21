using TicTacToe.Domain.Exceptions;
using TicTacToe.Domain.Interface;
using TicTacToe.Domain.Model;

namespace TicTacToe.Domain.Service;

public class UserService(IUserRepository users) : IUserService
{
    public bool Register(SignUpRequest signUpRequest)
    {
        if (users.ExistsByLogin(signUpRequest.Login)) throw new UserAlreadyExistsException($"User with login '{signUpRequest.Login}' already exists.");
        users.Register(new User(Guid.NewGuid(), signUpRequest.Login), signUpRequest.Password);
        return true;
    }

    public Guid? Authenticate(SignUpRequest signUpRequest)
    {
        return users.TryAuthenticate(signUpRequest.Login, signUpRequest.Password, out var id) ? id : null;
    }

    public User? GetById(Guid id) => users.GetById(id);
}