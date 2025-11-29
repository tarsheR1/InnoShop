using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken);
        Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken);
        Task<Role?> GetDefaultRoleAsync(CancellationToken cancellationToken);
    }
}
