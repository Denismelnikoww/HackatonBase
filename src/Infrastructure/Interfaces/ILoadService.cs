using Domain.Health;

namespace Infrastructure.Interfaces
{
    public interface ILoadService
    {
        Task<LoadInfo> CurrentLoad();
        Task<LoadInfo> SnapshotLoad();
    }
}