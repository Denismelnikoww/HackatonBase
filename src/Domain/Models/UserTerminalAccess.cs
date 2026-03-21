namespace Domain.Models;

public class UserTerminalAccess
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid TerminalId { get; set; }
    public Terminal Terminal { get; set; }
}