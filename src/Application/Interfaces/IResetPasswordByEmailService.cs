using ResultSharp.Core;

namespace Infrastructure.Interfaces
{
    public interface IResetPasswordByEmailService
    {
        Task<Result> ResetPassword(string email, string password, CancellationToken ct);
        Task<Result> SendLink(string email, CancellationToken ct);
    }
}