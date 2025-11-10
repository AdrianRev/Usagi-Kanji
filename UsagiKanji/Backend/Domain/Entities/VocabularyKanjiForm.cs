namespace Domain.Entities
{
    public class VocabularyKanjiForm : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public bool Common { get; set; }

        public Guid VocabularyId { get; set; }
        public Vocabulary Vocabulary { get; set; } = null!;

        public ICollection<VocabularyKanjiCharacter> KanjiCharacters { get; set; } = new List<VocabularyKanjiCharacter>();
    }
}