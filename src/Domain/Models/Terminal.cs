using Domain.Models.Common;

namespace Domain.Models
{
    public class Terminal : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
