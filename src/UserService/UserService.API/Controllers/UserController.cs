using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Dto.Common;
using UserService.API.Dto.User;
using UserService.API.Extensions;
using UserService.Application.Dto.UserDto;
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

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserId();
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
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(
            Guid id,
            CancellationToken cancellationToken)
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

        [HttpPut("me")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateCurrentUser(
            [FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserId();
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

        [HttpPut("me/password")]
        public async Task<ActionResult<ApiResponse>> UpdatePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserId();
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

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse>> UpdateUserStatus(
            Guid id,
            [FromBody] UpdateUserStatusRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (request.IsActive)
                {
                    await _userService.ActivateUserAsync(id, cancellationToken);
                    return Ok(new ApiResponse { Message = "User activated successfully" });
                }
                else
                {
                    await _userService.DeactivateUserAsync(id, cancellationToken);
                    return Ok(new ApiResponse { Message = "User deactivated successfully" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }

        [HttpGet("validation/email")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckEmailAvailability(
            [FromQuery] string email,
            CancellationToken cancellationToken)
        {
            try
            {
                var isAvailable = await _userService.IsEmailUniqueAsync(email, cancellationToken);
                return Ok(new ApiResponse<bool> { Data = isAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool> { Success = false, Error = ex.Message });
            }
        }
    }
}
