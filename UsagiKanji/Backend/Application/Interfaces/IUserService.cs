using Domain.Entities;
using FluentResults;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> SignUpAsync(string username, string email, string password);
        Task<Result<string>> LoginAsync(string usernameOrEmail, string password);
    }
}
