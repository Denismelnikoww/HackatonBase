namespace Infrastructure.Interfaces
{
    public interface IVerificationCacheProvider
    {
        Task<int> GenerateToken(string identifier, CancellationToken ct);
        Task<bool> ValidateToken(string identifier, string token, CancellationToken ct);
    }
}