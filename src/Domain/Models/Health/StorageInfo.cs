namespace Domain.Health
{
    public class StorageInfo
    {
        public List<DriveInfo> Drives { get; set; } = new List<DriveInfo>();

        public long TotalBytes => Drives.Sum(d => d.TotalBytes);
        public long UsedBytes => Drives.Sum(d => d.UsedBytes);

        public double TotalUsagePercentage =>
            TotalBytes > 0 ? (UsedBytes * 100.0 / TotalBytes) : 0;

    }
}
