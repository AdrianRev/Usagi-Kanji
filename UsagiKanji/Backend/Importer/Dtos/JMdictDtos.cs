namespace Importer.Dtos
{
    public class VocabularyRoot
    {
        public List<VocabularyWordDto> Words { get; set; } = new();
    }

    public class VocabularyWordDto
    {
        public string Id { get; set; } = string.Empty;
        public List<KanjiVariantDto> Kanji { get; set; } = new();
        public List<KanaDto> Kana { get; set; } = new();
        public List<SenseDto> Sense { get; set; } = new();
    }

    public class KanjiVariantDto
    {
        public string Text { get; set; } = string.Empty;
        public bool Common { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public class KanaDto
    {
        public string Text { get; set; } = string.Empty;
        public bool Common { get; set; }
        public List<string> Tags { get; set; } = new();
        public List<string> AppliesToKanjiForm { get; set; } = new();
    }

    public class SenseDto
    {
        public List<string> PartOfSpeech { get; set; } = new();
        public List<string> AppliesToKanjiForm { get; set; } = new();
        public List<string> AppliesToKana { get; set; } = new();
        public List<GlossDto> Gloss { get; set; } = new();
    }

    public class GlossDto
    {
        public string Lang { get; set; } = "eng";
        public string Text { get; set; } = string.Empty;
    }
}
