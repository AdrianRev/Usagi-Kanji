namespace Domain.Entities
{
    public class VocabularyKanjiCharacter : BaseEntity
    {
        public Guid VocabularyKanjiFormId { get; set; }
        public VocabularyKanjiForm KanjiForm { get; set; } = null!;

        public Guid KanjiId { get; set; }
        public Kanji Kanji { get; set; } = null!;

        public string? Reading { get; set; }
    }
}
