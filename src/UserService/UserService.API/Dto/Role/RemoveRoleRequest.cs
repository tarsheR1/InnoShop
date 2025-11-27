namespace UserService.API.Dto.Role
{
    public class RemoveRoleRequest
    {
        public Guid UserId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
