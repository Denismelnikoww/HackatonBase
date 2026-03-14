namespace Infrastructure.Interfaces
{
    public interface IResetPasswordService
    {
        Task ResetPassword(string email, string token, string password, CancellationToken ct);
        Task SendLink(string email, CancellationToken ct);
    }
}