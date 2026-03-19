namespace Application.Interfaces
{
    public interface IEmailConfirmService
    {
        Task ConfirmEmail(string emailId, CancellationToken ct);
        Task SendLink(Guid userId, CancellationToken ct);
        Task SendLink(string email, CancellationToken ct);
    }
}