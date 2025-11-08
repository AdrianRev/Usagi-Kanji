namespace Application.Dtos
{
    public class KanjiListItemDto
    {
        public Guid Id { get; set; }
        public string Character { get; set; } = string.Empty;
        public string? PrimaryMeaning { get; set; }
    }
}
