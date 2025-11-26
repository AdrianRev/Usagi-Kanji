namespace Application.Dtos
{
    public class ReviewQueueDto
    {
        public Guid KanjiId { get; set; }
        public string Character { get; set; } = string.Empty;
        public int Interval { get; set; }
        public string? Notes { get; set; }
        public string? Keyword { get; set; }
    }

    public class SubmitBatchReviewDto
    {
        public List<KanjiReviewItem> Reviews { get; set; } = new();
    }

    public class KanjiReviewItem
    {
        public Guid KanjiId { get; set; }
        public string Rating { get; set; } = string.Empty;
    }

}
