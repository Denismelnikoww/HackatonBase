namespace Web.Contracts.Responses
{
    public class PagedResponse<T>
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
        public List<T> Items { get; set; } = new List<T>();
    }
}
