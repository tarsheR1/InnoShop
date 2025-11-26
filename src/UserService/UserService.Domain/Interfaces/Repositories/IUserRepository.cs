using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken);
        Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken);
    }
}
