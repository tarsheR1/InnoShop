namespace UserService.API.Dto
{
    public class RemoveRoleRequest
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
