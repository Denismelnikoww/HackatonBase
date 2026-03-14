namespace Infrastructure.Interfaces
{
    public interface IEmailService
    {
        void Dispose();
        Task SendAsync(string email, string subject, string text, CancellationToken ct = default);
    }
}