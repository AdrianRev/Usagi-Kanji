namespace Domain.Entities
{
    public class Kanji : BaseEntity
    {
        public string Character { get; set; } = string.Empty;
        public int? StrokeCount { get; set; }
        public int? Grade { get; set; }
        public int? SortIndex_Grade { get; set; }
        public int? SortIndex_JLPT { get; set; }
        public int? JLPTLevel { get; set; }
        public int? FrequencyRank { get; set; }
        public int? HeisigNumber { get; set; }
        public int? Heisig6Number { get; set; }
        
        

        public ICollection<Reading> Readings { get; set; } = new List<Reading>();
        public ICollection<Meaning> Meanings { get; set; } = new List<Meaning>();
        public ICollection<VocabularyKanjiCharacter> VocabularyKanjiCharacters { get; set; } = new List<VocabularyKanjiCharacter>();
        public ICollection<UserKanji> UserKanjis { get; set; } = new List<UserKanji>();

    }
}
