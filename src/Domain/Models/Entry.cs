using Domain.Models.Common;

namespace Domain.Models
{
    public class Entry :IIdEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Time { get; set; }

        public Guid UserId { get; set; }
        public Guid TerminalId { get; set; }

        public virtual Terminal Terminal { get; set; }
        public virtual User User { get; set; }
    }
}
