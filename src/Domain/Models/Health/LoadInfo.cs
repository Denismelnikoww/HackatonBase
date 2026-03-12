namespace Domain.Health
{
    public class LoadInfo
    {
        public DateTime Timestamp { get; set; }
        public RamInfo RAM { get; set; }
        public StorageInfo Storage { get; set; }
        public List<CpuCoreInfo> CPU { get; set; }
        public SystemInfo System { get; set; }
    }
}
