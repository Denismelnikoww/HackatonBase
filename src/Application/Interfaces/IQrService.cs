namespace Application.Interfaces;

public interface IQrService
{
    Task<Guid> Generate(Guid userId, CancellationToken ct = default);
}