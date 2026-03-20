namespace Application.DTO
{
    public class EntryDto
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public Guid UserId { get; set; }
        public Guid TerminalId { get; set; }
    }
}
