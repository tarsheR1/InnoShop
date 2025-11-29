using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Models.Dto.Auth
{
    public class ConfirmationEmailRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
