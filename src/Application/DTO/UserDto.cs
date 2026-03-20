using Domain.Models;

namespace Application.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string? Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public Role Role { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsBanned { get; set; }
    }
}
