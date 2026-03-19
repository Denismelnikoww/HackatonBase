namespace Infrastructure.Interfaces
{
    public interface IResetPasswordByEmailService
    {
        Task ResetPassword(string email, string password, CancellationToken ct);
        Task SendLink(string email, CancellationToken ct);
    }
}