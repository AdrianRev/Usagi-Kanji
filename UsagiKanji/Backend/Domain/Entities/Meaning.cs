namespace Domain.Entities
{
    public class Meaning : BaseEntity
    {
        public string? Language { get; set; } = "en";
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
        public Guid KanjiId { get; set; }
        public Kanji Kanji { get; set; } = null!;
    }
}
