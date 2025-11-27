namespace UserService.API.Dto
{
    public UserDto MapToUserDto(Domain.Entities.User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.Name,
            CreatedAt = user.CreatedAt
        };
    }
}
