namespace Application.DTO;

public class UserWithTerminalsDto
{
    public Guid Id { get; set; }
    public bool EntryAccess { get; set; }
    public IEnumerable<Guid> Terminals { get; set; }
}