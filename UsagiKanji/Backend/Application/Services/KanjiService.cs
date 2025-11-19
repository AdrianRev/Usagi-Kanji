using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Domain.Utilities;
using FluentResults;

namespace Application.Services
{
    public class KanjiService : IKanjiService
    {
        private readonly IKanjiRepository _kanjiRepository;
        public KanjiService(IKanjiRepository kanjiRepository)
        {
            _kanjiRepository = kanjiRepository;
        }

        public async Task<Result<PaginatedList<KanjiListItemDto>>> GetAllKanjiAsync(KanjiListParams parameters, Guid userId, CancellationToken cancellationToken = default)
        {
            var kanjiPage = await _kanjiRepository.GetAllKanjiAsync(
                parameters.PageIndex,
                parameters.PageSize,
                parameters.SortBy,
                cancellationToken);

            var kanjiIdsInPage = kanjiPage.Items.Select(k => k.Id).ToList();
            var learnedKanjiIds = await _kanjiRepository.GetLearnedKanjiIdsForUserAsync(userId, kanjiIdsInPage, cancellationToken);

            var dtoPage = new PaginatedList<KanjiListItemDto>(
                kanjiPage.Items.Select(k => new KanjiListItemDto
                {
                    Id = k.Id,
                    Character = k.Character,
                    PrimaryMeaning = k.Meanings?.FirstOrDefault(m => m.IsPrimary)?.Value,
                    IsLearned = learnedKanjiIds.Contains(k.Id)
                }).ToList(),
                kanjiPage.PageIndex,
                kanjiPage.TotalPages
            );

            return Result.Ok(dtoPage);
        }

        public async Task<KanjiDetailsDto?> GetKanjiByIdAsync(Guid kanjiId, Guid userId)
        {
            var kanji = await _kanjiRepository.GetKanjiWithUserDetailsAsync(kanjiId, userId);
            if (kanji == null) return null;

            // Filter UserKanjis by userId
            var userKanji = kanji.UserKanjis?.FirstOrDefault(uk => uk.UserId == userId);

            var vocabularies = kanji.VocabularyKanjiCharacters?
                .Select(vkc => vkc.KanjiForm?.Vocabulary)
                .Where(v => v != null)
                .Distinct()
                .ToList() ?? new List<Domain.Entities.Vocabulary>();

            return new KanjiDetailsDto
            {
                Id = kanjiId,
                Character = kanji.Character,
                StrokeCount = kanji.StrokeCount,
                Grade = kanji.Grade,
                JLPTLevel = kanji.JLPTLevel,
                FrequencyRank = kanji.FrequencyRank,
                HeisigNumber = kanji.HeisigNumber,
                Heisig6Number = kanji.Heisig6Number,
                Readings = kanji.Readings?
                    .Select(r => new ReadingDto
                    {
                        Value = r.Value,
                        Type = r.Type.ToString()
                    })
                    .ToList() ?? new List<ReadingDto>(),
                Meanings = kanji.Meanings?
                    .Select(m => new MeaningDto
                    {
                        Value = m.Value,
                        IsPrimary = m.IsPrimary
                    })
                    .ToList() ?? new List<MeaningDto>(),
                Notes = userKanji?.Notes,
                Keyword = userKanji?.Keyword,
                IsLearned = userKanji != null,
                Vocabulary = vocabularies.Select(v =>
                {
                    var mainForm = v?.KanjiForms?.FirstOrDefault();
                    var kanaReadings = v?.KanaReadings?
                        .Where(k => (k.AppliesToKanji == "*" || k.AppliesToKanji.Contains(kanji.Character))
                            && k.Common)
                        .Select(k => k.Text)
                        .ToList() ?? new List<string>();
                    return new VocabularyDto
                    {
                        Text = mainForm?.Text ?? string.Empty,
                        Common = mainForm?.Common ?? false,
                        KanaReadings = kanaReadings,
                        Glosses = v?.Glosses?.Select(g => g.Text).ToList() ?? new List<string>()
                    };
                }).ToList()
            };
        }

        public async Task<Result> UpdateOrAddUserKanjiAsync(Guid kanjiId, Guid userId, UpdateOrAddUserKanjiDto dto)
        {
            var userKanji = await _kanjiRepository.GetUserKanjiAsync(userId, kanjiId);

            if (userKanji == null)
            {
                userKanji = new UserKanji
                {
                    KanjiId = kanjiId,
                    UserId = userId,
                    Notes = dto.Notes,
                    Keyword = dto.Keyword
                };
                await _kanjiRepository.AddAsync(userKanji);
            }
            else
            {
                userKanji.Notes = dto.Notes;
                userKanji.Keyword = dto.Keyword;
                await _kanjiRepository.UpdateAsync(userKanji);
            }

            await _kanjiRepository.SaveChangesAsync();
            return Result.Ok();
        }

    }
}
