using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Domain.Interfaces.Services;

namespace UserService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterUserAsync(string email, string password, string? firstName, string? lastName, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Users.EmailExistsAsync(email, cancellationToken))
                throw new InvalidOperationException($"Email '{email}' is already registered");

            var defaultRole = await _unitOfWork.Roles.GetDefaultRoleAsync(cancellationToken)
                ?? throw new InvalidOperationException("Default role not found");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = _passwordHasher.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                RoleId = defaultRole.Id,
                IsActive = false, // До тех пора пока почта не подтверждена
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        }

        public async Task UpdateUserProfileAsync(Guid userId, string? firstName, string? lastName, string? email, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (!string.IsNullOrEmpty(email) && email != user.Email)
            {
                if (await _unitOfWork.Users.EmailExistsAsync(email, cancellationToken))
                    throw new InvalidOperationException($"Email '{email}' is already taken");

                user.Email = email;
                user.EmailConfirmed = false; 
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("User not found");

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, currentPassword))
                throw new InvalidOperationException("Current password is incorrect");

            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ActivateUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Users.GetUsersByRoleAsync(roleName, cancellationToken);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken)
        {
            return !await _unitOfWork.Users.EmailExistsAsync(email, cancellationToken);
        }
    }
}
