using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Dto.Auth;
using UserService.API.Dto.Common;
using UserService.API.Extensions;
using UserService.Application.Dto.UserDto;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces.Services;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("tokens")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.AuthenticateAsync(request.Username, request.Password, cancellationToken);

                if (!result.Result)
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Error = result.ErrorMessage
                    });

                var authResponse = new AuthResponse
                {
                    AccessToken = result.AccessToken!,
                    RefreshToken = result.RefreshToken!,
                    ExpiresAt = result.ExpiresAt!.Value,
                    User = new UserDto
                    {
                        Id = result.User!.Id,
                        Email = result.User.Email,
                        FirstName = result.User.FirstName,
                        LastName = result.User.LastName,
                        Role = result.User.Role.Name,
                        CreatedAt = result.User.CreatedAt
                    }
                };

                return Ok(new ApiResponse<AuthResponse> { Data = authResponse });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        [HttpPost("users")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Register(
            [FromBody] RegisterRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(
                    request.Email,
                    request.Password,
                    request.FirstName,
                    request.LastName,
                    cancellationToken);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.Name,
                    CreatedAt = user.CreatedAt
                };

                return Ok(new ApiResponse<UserDto> { Data = userDto });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UserDto>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        [HttpPut("tokens")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(
            [FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, cancellationToken);

                if (!result.Result)
                    return BadRequest(new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        Error = result.ErrorMessage
                    });

                var authResponse = new AuthResponse
                {
                    AccessToken = result.AccessToken!,
                    RefreshToken = result.RefreshToken!,
                    ExpiresAt = result.ExpiresAt!.Value,
                    User = new UserDto
                    {
                        Id = result.User!.Id,
                        Email = result.User.Email,
                        FirstName = result.User.FirstName,
                        LastName = result.User.LastName,
                        Role = result.User.Role.Name,
                        CreatedAt = result.User.CreatedAt
                    }
                };

                return Ok(new ApiResponse<AuthResponse> { Data = authResponse });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<AuthResponse>
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        [HttpDelete("tokens")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> Revoke(
            [FromBody] RevokeTokenRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _authService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
                return Ok(new ApiResponse { Message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }

        [HttpPost("tokens/all")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> Logout(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User.GetUserId();
                await _authService.RevokeAllUserTokensAsync(userId, cancellationToken);
                return Ok(new ApiResponse { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Success = false, Error = ex.Message });
            }
        }
    }
}
