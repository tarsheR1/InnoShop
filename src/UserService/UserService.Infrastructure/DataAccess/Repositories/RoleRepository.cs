using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repositories;

namespace UserService.Infrastructure.DataAccess.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(UsersDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        }

        public async Task<bool> RoleExistsAsync(string roleName, CancellationToken cancellationToken)
        {
            return await _dbSet.AnyAsync(r => r.Name == roleName, cancellationToken);
        }

        public async Task<Role?> GetDefaultRoleAsync(CancellationToken cancellationToken)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);
        }
    }
}
