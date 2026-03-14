namespace Infrastructure.Interfaces
{
    public interface IVerificationTokenProvider
    {
        Task<int> GenerateToken(string identifier, CancellationToken ct);
        Task<bool> ValidateToken(string identifier, string token, CancellationToken ct);
    }
}