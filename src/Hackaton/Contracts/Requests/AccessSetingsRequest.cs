namespace Web.Contracts.Requests;

public class AccessSetingsRequest
{
    public Guid UserId { get; set; }
    public bool EntryAccess { get; set; }
    public IEnumerable<Guid> Terminals { get; set; }
}