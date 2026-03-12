namespace Domain.Health
{
    public class RamInfo
    {
        public long TotalBytes { get; set; }
        public long UsedBytes { get; set; }
        public string MemoryType { get; set; }
        public double UsagePercentage =>
            TotalBytes > 0 ? (UsedBytes * 100.0 / TotalBytes) : 0;
    }
}
