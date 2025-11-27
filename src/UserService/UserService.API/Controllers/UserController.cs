using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Dto;
using UserService.Application.Dto.User;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Services;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile(CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.GetUserByIdAsync(userId, cancellationToken);

                if (user == null)
                    return NotFound(new ApiResponse<UserDto> { Success = false, Error = "User not found" });

                var userDto = _mapper.Map<UserDto>(user);
                return Ok(new ApiResponse<UserDto> { Data = userDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id, cancellationToken);

                if (user == null)
                    return NotFound(new ApiResponse<UserDto> { Success = false, Error = "User not found" });

                var userDto = _mapper.Map<UserDto>(user);
                return Ok(new ApiResponse<UserDto> { Data = userDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile(
            [FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _userService.UpdateUserProfileAsync(
                    userId,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    cancellationToken);

                var updatedUser = await _userService.GetUserByIdAsync(userId, cancellationToken);
                var userDto = _mapper.Map<UserDto>(updatedUser!);

                return Ok(new ApiResponse<UserDto> { Data = userDto, Message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserDto> { Success = false, Error = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _userService.ChangePasswordAsync(
                    userId,
                    request.CurrentPassword,
                    request.NewPassword,
                    cancellationToken);

                return Ok(new ApiResponse { Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetUsers(
            [FromQuery] string? role,
            CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<User> users;

                if (!string.IsNullOrEmpty(role))
                {
                    users = await _userService.GetUsersByRoleAsync(role, cancellationToken);
                }
                else
                {
                    users = await _userService.GetUsersByRoleAsync("User", cancellationToken);
                    var admins = await _userService.GetUsersByRoleAsync("Admin", cancellationToken);
                    var moderators = await _userService.GetUsersByRoleAsync("Moderator", cancellationToken);

                    users = users.Concat(admins).Concat(moderators);
                }

                var userDtos = users.Select(_mapper.Map<UserDto>).ToList();
                return Ok(new ApiResponse<List<UserDto>> { Data = userDtos });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<UserDto>> { Success = false, Error = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> DeactivateUser(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeactivateUserAsync(id, cancellationToken);
                return Ok(new ApiResponse { Message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }

        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> ActivateUser(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.ActivateUserAsync(id, cancellationToken);
                return Ok(new ApiResponse { Message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("check-email")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmailExists(
            [FromQuery] string email,
            CancellationToken cancellationToken)
        {
            try
            {
                var isUnique = await _userService.IsEmailUniqueAsync(email, cancellationToken);
                return Ok(new ApiResponse<bool> { Data = isUnique });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool> { Success = false, Error = ex.Message });
            }
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");

            return Guid.Parse(userIdClaim);
        }
    }

}
