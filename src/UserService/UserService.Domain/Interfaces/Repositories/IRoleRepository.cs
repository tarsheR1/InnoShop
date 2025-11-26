using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role?> GetByNameAsync(string roleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<Role?> GetDefaultRoleAsync();
    }
}
