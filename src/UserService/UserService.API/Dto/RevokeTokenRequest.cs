namespace UserService.API.Dto
{
    public class RevokeTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
