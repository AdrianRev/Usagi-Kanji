using System;

namespace Application.Dtos
{
    public class UserKanjiDto
    {
        public Guid KanjiId { get; set; }
        public string Character { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Keyword { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public int Interval { get; set; } // Current SRS interval in days
    }
}
