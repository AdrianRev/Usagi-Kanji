
namespace Application.Dtos
{
    public class NextUnlearnedKanjiDto
    {
        public Guid Id { get; set; }
        public string Character { get; set; } = string.Empty;
        public string? PrimaryMeaning { get; set; }
        public bool IsLearned { get; set; }
        public int? CurrentIndex { get; set; }
    }
}
