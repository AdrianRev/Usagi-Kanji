namespace Domain.Entities
{
    public class VocabularyKana : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public bool Common { get; set; }

        public string AppliesToKanjiForm { get; set; } = "*";

        public Guid VocabularyId { get; set; }
        public Vocabulary Vocabulary { get; set; } = null!;
    }
}

