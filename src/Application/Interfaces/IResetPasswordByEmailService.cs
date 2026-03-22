namespace Infrastructure.Interfaces
{
    public interface IResetPasswordByEmailService
    {
        Task SendPassword(string email, CancellationToken ct);
    }
}