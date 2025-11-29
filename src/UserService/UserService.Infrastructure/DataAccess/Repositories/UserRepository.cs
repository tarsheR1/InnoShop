using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(UsersDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(u => u.Role)
                .Where(u => u.Role.Name == roleName && u.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public async Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token, cancellationToken);
        }

        public async Task UpdateLastLoginAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _dbSet.FindAsync(new object[] { userId }, cancellationToken);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                _dbSet.Update(user);
            }
        }
    }
}
