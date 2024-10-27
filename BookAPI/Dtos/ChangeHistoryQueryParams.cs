namespace BookAPI.Dtos
{
    public class ChangeHistoryQueryParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Sort { get; set; }
        public string? Filter { get; set; }
        public string? GroupBy { get; set; }
    }
}
