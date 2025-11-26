using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Services
{
    public interface IRoleService
    {
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<bool> UserHasRoleAsync(int userId, string roleName);
        Task<bool> AssignRoleToUserAsync(int userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(int userId, string roleName);
        Task<string[]> GetUserRolesAsync(int userId);
    }
}
