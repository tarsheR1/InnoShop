using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Domain.Interfaces.Services;

namespace UserService.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Roles.GetByNameAsync(roleName, cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync(CancellationToken cancellationToken)
        {
            return await _unitOfWork.Roles.GetAllAsync(cancellationToken);
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return false;

            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId, cancellationToken);
            return role?.Name == roleName;
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return false;

            var role = await _unitOfWork.Roles.GetByNameAsync(roleName, cancellationToken);
            if (role == null)
                return false;

            user.RoleId = role.Id;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return false;

            var currentRole = await _unitOfWork.Roles.GetByIdAsync(user.RoleId, cancellationToken);
            if (currentRole?.Name != roleName)
                return false; 

            var defaultRole = await _unitOfWork.Roles.GetDefaultRoleAsync(cancellationToken);
            if (defaultRole == null)
                return false;

            user.RoleId = defaultRole.Id;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return new List<Role>();

            var role = await _unitOfWork.Roles.GetByIdAsync(user.RoleId, cancellationToken);
            return role != null ? new List<Role> { role } : new List<Role>();
        }
    }

}
