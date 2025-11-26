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


        public async Task<Result> SubmitReviewAsync(Guid userId, Guid kanjiId, string rating)
        {
            var userKanji = await _kanjiRepository.GetUserKanjiAsync(userId, kanjiId);
            if (userKanji == null)
                return Result.Fail("UserKanji not found");

            DateTime today = DateTime.UtcNow.Date;

            switch (rating)
            {
                case "Easy":
                    userKanji.Interval += 2;
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;

                case "Good":
                    userKanji.Interval += 1;
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;

                case "Hard":
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;

                case "Again":
                    userKanji.Interval = Math.Max(0, userKanji.Interval - 2);
                    userKanji.NextReviewDate = today;
                    break;

                default:
                    userKanji.Interval += 1;
                    userKanji.NextReviewDate = today.AddDays(GetIntervalDays(userKanji.Interval));
                    break;
            }

            await _kanjiRepository.UpdateAsync(userKanji);
            await _kanjiRepository.SaveChangesAsync();

            return Result.Ok();
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
