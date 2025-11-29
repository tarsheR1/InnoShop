using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Dto.Common;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Services;

namespace UserService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailManagementService _emailManagementService;

        public EmailController(IUserService userService, IEmailManagementService emailService)
        {
            _userService = userService;
            _emailManagementService = emailService;
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse>> ForgotPassword(
            [FromBody] ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            await _emailManagementService.ForgotPasswordAsync(request.Email, cancellationToken);
            return Ok(new ApiResponse { Message = "Если email существует, инструкции отправлены" });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword(
            [FromBody] ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.)
                return BadRequest(new ApiResponse { Success = false, Error = "Пароли не совпадают" });

            var result = await _userService.ResetPasswordAsync(
                request.Token, request.NewPassword, cancellationToken);

            if (!result)
                return BadRequest(new ApiResponse { Success = false, Error = "Неверный или просроченный токен" });

            return Ok(new ApiResponse { Message = "Пароль успешно изменен" });
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult<ApiResponse>> ConfirmEmail(
            [FromBody] ConfirmEmailRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.ConfirmEmailAsync(
                request.Email, request.Token, cancellationToken);

            if (!result)
                return BadRequest(new ApiResponse { Success = false, Error = "Неверный или просроченный токен" });

            return Ok(new ApiResponse { Message = "Email успешно подтвержден" });
        }

        [HttpPost("resend-confirmation")]
        public async Task<ActionResult<ApiResponse>> ResendConfirmation(
            [FromBody] ResendConfirmationRequest request,
            CancellationToken cancellationToken)
        {
            await _userService.ResendEmailConfirmationAsync(request.Email, cancellationToken);
            return Ok(new ApiResponse { Message = "Письмо с подтверждением отправлено" });
        }

        [HttpGet("email-confirmed")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> IsEmailConfirmed(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var isConfirmed = await _.IsEmailConfirmedAsync(userId, cancellationToken);
            return Ok(new ApiResponse<bool> { Data = isConfirmed });
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
