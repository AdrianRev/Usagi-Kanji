using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task AddAsync(User user);
    }
}
