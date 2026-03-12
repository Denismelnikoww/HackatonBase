namespace Domain.Health
{
    public class DriveInfo
    {
        public string Name { get; set; }
        public string FileSystem { get; set; }
        public string DriveType { get; set; }
        public long TotalBytes { get; set; }
        public long UsedBytes { get; set; }
        public double UsagePercentage =>
            TotalBytes > 0 ? (UsedBytes * 100.0 / TotalBytes) : 0;
    }
}
