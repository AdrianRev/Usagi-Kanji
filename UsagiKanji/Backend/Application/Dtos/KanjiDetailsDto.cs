namespace Application.Dtos
{
    public class KanjiDetailsDto
    {
        public string Character { get; set; } = string.Empty;
        public int? StrokeCount { get; set; }
        public int? Grade { get; set; }
        public int? JLPTLevel { get; set; }
        public int? FrequencyRank { get; set; }
        public int? HeisigNumber { get; set; }
        public int? Heisig6Number { get; set; }

        public ICollection<ReadingDto> Readings { get; set; } = new List<ReadingDto>();
        public ICollection<MeaningDto> Meanings { get; set; } = new List<MeaningDto>();
        public ICollection<VocabularyDto> Vocabulary { get; set; } = new List<VocabularyDto>();

        public string? Notes { get; set; }
        public string? Keyword { get; set; }
        public bool IsLearned { get; set; }
    }
    public class ReadingDto
    {
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    public class MeaningDto
    {
        public string Value { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
    public class VocabularyDto
    {
        public string Text { get; set; } = string.Empty;
        public bool Common { get; set; }
        public List<string> KanaReadings { get; set; } = new();
        public List<string> Glosses { get; set; } = new();
    }
}
