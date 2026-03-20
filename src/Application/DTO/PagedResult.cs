namespace Application.DTO
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Total { get; set; } = 0;
        public int Take { get; set; } = 0;
        public int Skip { get; set; } = 0;
    }
}
