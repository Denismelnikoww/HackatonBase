using Domain.Models.Common;

namespace Domain.Models.User
{
    public class Session : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }

        public DateTime LoginDate { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        public DateTime? LogoutDate { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public virtual User User { get; set; }
        public virtual Token Token { get; set; }
    }
}
