namespace Domain.Entities
{
    public class VocabularyGloss : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public string Language { get; set; } = "eng";

        public Guid VocabularyId { get; set; }
        public Vocabulary Vocabulary { get; set; } = null!;
    }
}

