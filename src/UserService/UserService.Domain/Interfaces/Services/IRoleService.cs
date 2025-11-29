using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Services
{
    public interface IRoleService
    {
        Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken);
        Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken);
        Task<bool> UserHasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken);
        Task<bool> AssignRoleToUserAsync(Guid userId, string roleName, CancellationToken cancellationToken);
        Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName, CancellationToken cancellationToken);
        Task<List<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);
    }
}
