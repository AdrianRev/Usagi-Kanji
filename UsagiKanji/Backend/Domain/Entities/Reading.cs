using Domain.Enums;

namespace Domain.Entities
{
    public class Reading : BaseEntity
    {
        public ReadingType Type { get; set; }
        public string Value { get; set; } = string.Empty;

        public Guid KanjiId { get; set; }
        public Kanji Kanji { get; set; } = null!;
    }
}
