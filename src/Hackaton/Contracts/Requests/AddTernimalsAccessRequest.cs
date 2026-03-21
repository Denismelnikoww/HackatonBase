namespace Web.Contracts.Requests;

public class AddTernimalsAccessRequest
{
    public Guid UserId { get; set; }
    public IEnumerable<Guid> Terminals { get; set; }
}