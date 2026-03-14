using Domain.Models.Common;

namespace Domain.Models.User
{
    public class Token : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SessionId { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public virtual Session Session { get; set; }
    }
}