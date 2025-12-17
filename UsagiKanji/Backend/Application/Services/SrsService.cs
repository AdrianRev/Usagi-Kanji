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
                    continue; // or collect errors

                ApplyRating(userKanji, review.Rating, today);
                await _kanjiRepository.UpdateAsync(userKanji);
            }

            await _kanjiRepository.SaveChangesAsync(); // One save at the end!
            return Result.Ok();
        }

        private void ApplyRating(UserKanji userKanji, string rating, DateTime today)
        {
            switch (rating)
            {
                case "Again":
                    userKanji.Interval = Math.Max(0, userKanji.Interval - 2);
                    userKanji.NextReviewDate = today;
                    break;

                case "Hard":
                    userKanji.Interval = Math.Max(1, userKanji.Interval);
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;

                case "Good":
                    userKanji.Interval += 1;
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;

                case "Easy":
                    userKanji.Interval += 2;
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;

                default:
                    userKanji.Interval += 1;
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;
            }
        }


        private int GetIntervalDays(int interval)
        {
            return interval switch
            {
                0 => 0,
                1 => 1,
                2 => 3,
                3 => 7,
                4 => 14,
                5 => 30,
                6 => 60,
                _ => 0
            };
        }
    }
}
