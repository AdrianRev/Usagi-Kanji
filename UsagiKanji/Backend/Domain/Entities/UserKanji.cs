namespace Domain.Entities
{
    public class UserKanji : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid KanjiId { get; set; }

        public string? Notes { get; set; }
        public string? Keyword { get; set; }

        public double EaseFactor { get; set; } = 2.5;
        public int Interval { get; set; } = 0;
        public DateTime? NextReviewDate { get; set; }

        public Kanji Kanji { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
