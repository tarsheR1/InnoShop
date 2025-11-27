using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(string email, string password, string? firstName, string? lastName, CancellationToken cancellationToken);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
        Task UpdateUserProfileAsync(Guid userId, string? firstName, string? lastName, string? email, CancellationToken cancellationToken);
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
        Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken);
        Task ActivateUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken);
        Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken);
    }
}
