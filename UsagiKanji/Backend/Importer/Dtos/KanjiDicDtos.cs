namespace Importer.Dtos
{
    public class KanjiDicRoot
    {
        public string Version { get; set; } = string.Empty;
        public List<KanjiDicCharacter> Characters { get; set; } = new();
    }

    public class KanjiDicCharacter
    {
        public string Literal { get; set; } = string.Empty;
        public KanjiMisc Misc { get; set; } = new();
        public KanjiReadingMeaning ReadingMeaning { get; set; } = new();
        public List<DictionaryReference>? DictionaryReferences { get; set; }
        public List<string>? Nanori { get; set; }
    }
    public class DictionaryReference
    {
        public string Type { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
    public class KanjiMisc
    {
        public int? Grade { get; set; }
        public List<int>? StrokeCounts { get; set; }
        public int? Frequency { get; set; }
        public int? JlptLevel { get; set; }
    }

    public class KanjiReadingMeaning
    {
        public List<KanjiGroup> Groups { get; set; } = new();
        public List<string>? Nanori { get; set; }
    }

    public class KanjiGroup
    {
        public List<KanjiReading> Readings { get; set; } = new();
        public List<KanjiMeaning> Meanings { get; set; } = new();
    }

    public class KanjiReading
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class KanjiMeaning
    {
        public string Lang { get; set; } = "en";
        public string Value { get; set; } = string.Empty;
    }
}
