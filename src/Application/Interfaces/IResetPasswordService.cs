namespace Infrastructure.Interfaces
{
    public interface IResetPasswordService
    {
        Task ResetPassword(Guid userId,string oldPassword, string newPassword, CancellationToken ct = default);
    }
}