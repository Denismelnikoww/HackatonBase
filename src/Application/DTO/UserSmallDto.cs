using Domain.Models;

namespace Application.DTO;

public class UserSmallDto
{
    public Guid Id { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool EntryAccess { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}