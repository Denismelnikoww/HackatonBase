namespace Infrastructure.Interfaces
{
    public interface IResetPasswordTokensProvider
    {
        Task<int> GetPasswordResetToken(string email);
        Task<bool> VerifyPasswordResetToken(string email, int inputToken);
    }
}