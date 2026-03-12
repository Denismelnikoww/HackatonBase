namespace Domain.Health
{
    public class SystemInfo
    {
        public string MachineName { get; set; }
        public string OS { get; set; }
        public int ProcessorCount { get; set; }
        public TimeSpan Uptime { get; set; }
        public string HostName { get; set; }
    }
}
