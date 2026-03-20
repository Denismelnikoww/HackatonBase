namespace Application.Interfaces;

public interface IQrService
{
    Task<Guid> Generate(Guid userId, CancellationToken ct = default);
    Task<Guid> Parse(Guid qrId, CancellationToken ct = default);
    Task Revoke(Guid qrId, CancellationToken ct = default);
}