using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(string email, string password, string? firstName, string? lastName);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserProfileAsync(Guid userId, string? firstName, string? lastName, string? email);
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task DeactivateUserAsync(Guid userId);
        Task ActivateUserAsync(Guid userId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
    }
}
