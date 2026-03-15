using ResultSharp.Core;

namespace Application.Interfaces
{
    public interface IEmailConfirmService
    {
        Task<Result> ConfirmEmail(string emailId, CancellationToken ct);
        Task<Result> SendLink(Guid userId, CancellationToken ct);
        Task<Result> SendLink(string email, CancellationToken ct);
    }
}