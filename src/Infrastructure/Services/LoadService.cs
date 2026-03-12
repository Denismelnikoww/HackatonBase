using Domain.Health;
using Hardware.Info;
using Infrastructure.Interfaces;

namespace Infrastructure.Services
{
    public class LoadService : ILoadService
    {
        private readonly IHardwareInfo _hardwareInfo;
        private LoadInfo _snapshot;
        private readonly object _syncLock = new();
        private DateTime _lastRefreshTime;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromSeconds(1);

        public LoadService()
        {
            _hardwareInfo = new HardwareInfo();
            Refresh();
        }

        private void Refresh()
        {
            lock (_syncLock)
            {
                _hardwareInfo.RefreshAll();
                _lastRefreshTime = DateTime.UtcNow;
            }
        }

        public async Task<LoadInfo> CurrentLoad()
        {
            await Task.Delay(100);

            lock (_syncLock)
            {
                if (DateTime.UtcNow - _lastRefreshTime > _refreshInterval)
                {
                    Refresh();
                }

                _snapshot = CreateLoadInfo();
                return _snapshot;
            }
        }

        public Task<LoadInfo> SnapshotLoad()
        {
            lock (_syncLock)
            {
                if (_snapshot == null)
                {
                    Refresh();
                    _snapshot = CreateLoadInfo();
                }
                return Task.FromResult(_snapshot);
            }
        }

        private LoadInfo CreateLoadInfo()
        {
            return new LoadInfo
            {
                Timestamp = _lastRefreshTime,
                RAM = GetRamInfo(),
                Storage = GetStorageInfo(),
                CPU = GetCpuInfo(),
                System = GetSystemInfo()
            };
        }

        private RamInfo GetRamInfo()
        {
            var memory = _hardwareInfo.MemoryStatus;
            return new RamInfo
            {
                TotalBytes = (long)memory.TotalPhysical,
                UsedBytes = (long)(memory.TotalPhysical - memory.AvailablePhysical),
                MemoryType = _hardwareInfo.MemoryList.FirstOrDefault().MemoryType.ToString(),
            };
        }

        private StorageInfo GetStorageInfo()
        {
            var storageInfo = new StorageInfo();

            foreach (var drive in _hardwareInfo.DriveList)
            {
                var partitions = drive.PartitionList
                    .Where(p => p.VolumeList.Any())
                    .SelectMany(p => p.VolumeList);

                foreach (var volume in partitions)
                {
                    storageInfo.Drives.Add(new Domain.Health.DriveInfo
                    {
                        Name = volume.Name,
                        FileSystem = volume.FileSystem,
                        DriveType = drive.Model,
                        TotalBytes = (long)volume.Size,
                        UsedBytes = (long)(volume.Size - volume.FreeSpace),
                    });
                }
            }
            return storageInfo;
        }

        private List<CpuCoreInfo> GetCpuInfo()
        {
            var cpuCores = new List<CpuCoreInfo>();

            foreach (var cpu in _hardwareInfo.CpuList)
            {
                for (int i = 0; i < cpu.CpuCoreList.Count; i++)
                {
                    var core = cpu.CpuCoreList[i];

                    cpuCores.Add(new CpuCoreInfo
                    {
                        CoreName = $"Core {i}",
                        UsagePercentage = core.PercentProcessorTime,
                    });
                }
            }
            return cpuCores;
        }

        private SystemInfo GetSystemInfo()
        {
            var cpu = _hardwareInfo.CpuList.FirstOrDefault();

            return new SystemInfo
            {
                MachineName = Environment.MachineName,
                OS = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64),
                HostName = Environment.MachineName
            };
        }
    }
}