namespace Application.Dtos
{
    public class KanjiListParams
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = null;
    }
}