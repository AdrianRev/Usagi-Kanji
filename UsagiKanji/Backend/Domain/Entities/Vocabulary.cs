namespace Domain.Entities
{
    public class Vocabulary : BaseEntity
    {
        public string JMdictId { get; set; } = string.Empty;
        public ICollection<VocabularyKanjiForm> KanjiForms { get; set; } = new List<VocabularyKanjiForm>();
        public ICollection<VocabularyKana> KanaReadings { get; set; } = new List<VocabularyKana>();
        public ICollection<VocabularyGloss> Glosses { get; set; } = new List<VocabularyGloss>();
    }

}
