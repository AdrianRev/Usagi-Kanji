using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Domain.Utilities;
using FluentResults;
using Application.Dtos;

namespace Application.Services
{
    public class SrsService : ISrsService
    {
        private readonly IKanjiRepository _kanjiRepository;

        public SrsService(IKanjiRepository kanjiRepository)
        {
            _kanjiRepository = kanjiRepository;
        }

        private readonly Dictionary<string, int> _ratingStageChange = new()
        {
            { "Easy", 2 },
            { "Good", 1 },
            { "Hard", 0 },
            { "Again", -2 }
        };

        public async Task<IReadOnlyList<UserKanjiDto>> GetDueKanjiAsync(Guid userId)
        {
            DateTime today = DateTime.UtcNow.Date;
            var dueKanjis = await _kanjiRepository.GetUserKanjisForUserDueAsync(userId, today);

            return dueKanjis.Select(uk => new UserKanjiDto
            {
                KanjiId = uk.KanjiId,
                Character = uk.Kanji.Character,
                Notes = uk.Notes,
                Keyword = uk.Keyword,
                NextReviewDate = uk.NextReviewDate,
                Interval = uk.Interval
            }).ToList();
        }


        public async Task<Result> SubmitBatchReviewAsync(Guid userId, SubmitBatchReviewDto dto)
        {
            if (!dto.Reviews.Any())
                return Result.Ok();

            DateTime today = DateTime.UtcNow.Date;

            foreach (var review in dto.Reviews)
            {
                var userKanji = await _kanjiRepository.GetUserKanjiAsync(userId, review.KanjiId);
                if (userKanji == null)
                    continue;

                ApplyRating(userKanji, review.Rating, today);
                await _kanjiRepository.UpdateAsync(userKanji);
            }

            await _kanjiRepository.SaveChangesAsync();
            return Result.Ok();
        }

        private void ApplyRating(UserKanji userKanji, string rating, DateTime today)
        {
            double ease = userKanji.EaseFactor;
            int quality = rating switch
            {
                "Again" => 0,
                "Hard" => 2,
                "Good" => 4,
                "Easy" => 5,
                _ => 4
            };

            if (quality < 3)
            {
                userKanji.Repetitions = 0;
                userKanji.Interval = 1;
                userKanji.Lapses++;
            }
            else
            {
                userKanji.Repetitions++;

                if (userKanji.Repetitions == 1)
                    userKanji.Interval = 1;
                else if (userKanji.Repetitions == 2)
                    userKanji.Interval = 6;
                else
                    userKanji.Interval = (int)Math.Round(userKanji.Interval * ease);
            }

            ease += 0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02);
            ease = Math.Max(1.3, ease);

            if (quality == 3)
                ease *= 0.9;
            else if (quality == 5)
                ease *= 1.2;

            userKanji.EaseFactor = ease;

            if (quality < 3)
                userKanji.NextReviewDate = today.AddDays(1);
            else
                userKanji.NextReviewDate = today.AddDays(userKanji.Interval);
        }
    }
}
