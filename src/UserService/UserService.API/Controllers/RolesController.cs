using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Dto.Common;
using UserService.Domain.Interfaces.Services;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Domain.Entities.Role>>>> GetRoles(CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync(cancellationToken);
                return Ok(new ApiResponse<List<Domain.Entities.Role>> { Data = roles.ToList() });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<Domain.Entities.Role>> { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("{roleName}")]
        public async Task<ActionResult<ApiResponse<Domain.Entities.Role>>> GetRoleByName(
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleService.GetRoleByNameAsync(roleName, cancellationToken);

                if (role == null)
                    return NotFound(new ApiResponse<Domain.Entities.Role> { Success = false, Error = "Role not found" });

                return Ok(new ApiResponse<Domain.Entities.Role> { Data = role });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<Domain.Entities.Role> { Success = false, Error = ex.Message });
            }
        }

        [HttpPut("users/{userId:guid}/roles/{roleName}")]
        public async Task<ActionResult<ApiResponse>> AssignRoleToUser(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _roleService.AssignRoleToUserAsync(userId, roleName, cancellationToken);

                if (!result)
                    return BadRequest(new ApiResponse { Success = false, Error = "Failed to assign role" });

                return Ok(new ApiResponse { Message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }

        [HttpDelete("users/{userId:guid}/roles/{roleName}")]
        public async Task<ActionResult<ApiResponse>> RemoveRoleFromUser(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _roleService.RemoveRoleFromUserAsync(userId, roleName, cancellationToken);

                if (!result)
                    return BadRequest(new ApiResponse { Success = false, Error = "Failed to remove role" });

                return Ok(new ApiResponse { Message = "Role removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("users/{userId:guid}")]
        public async Task<ActionResult<ApiResponse<List<Domain.Entities.Role>>>> GetUserRoles(
            Guid userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _roleService.GetUserRolesAsync(userId, cancellationToken);
                return Ok(new ApiResponse<List<Domain.Entities.Role>> { Data = roles });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<Domain.Entities.Role>> { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("users/{userId:guid}/roles/{roleName}")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckUserRole(
            Guid userId,
            string roleName,
            CancellationToken cancellationToken)
        {
            try
            {
                var hasRole = await _roleService.UserHasRoleAsync(userId, roleName, cancellationToken);
                return Ok(new ApiResponse<bool> { Data = hasRole });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool> { Success = false, Error = ex.Message });
            }
        }
    }
}