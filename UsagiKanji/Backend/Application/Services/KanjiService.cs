using Application.Dtos;
using Application.Interfaces;
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

        public async Task<Result<PaginatedList<KanjiListItemDto>>> GetAllKanjiAsync(
            KanjiListParams parameters,
            CancellationToken cancellationToken = default)
        {
            var kanjiPage = await _kanjiRepository.GetAllKanjiAsync(
                parameters.PageIndex, parameters.PageSize, parameters.SortBy, cancellationToken);

            var dtoPage = new PaginatedList<KanjiListItemDto>(
                kanjiPage.Items.Select(k => new KanjiListItemDto
                {
                    Id = k.Id,
                    Character = k.Character,
                    PrimaryMeaning = k.Meanings?.FirstOrDefault(m => m.IsPrimary)?.Value
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

            var userKanji = kanji.UserKanjis?.FirstOrDefault();

            var vocabularies = kanji.VocabularyKanjiCharacters?
                .Select(vkc => vkc.KanjiForm?.Vocabulary)
                .Where(v => v != null)
                .Distinct()
                .ToList() ?? new List<Domain.Entities.Vocabulary>();

            return new KanjiDetailsDto
            {
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
                    return new VocabularyDto
                    {
                        Text = mainForm?.Text ?? string.Empty,
                        Common = mainForm?.Common ?? false,
                        KanaReadings = v?.KanaReadings?.Select(k => k.Text).ToList() ?? new List<string>(),
                        Glosses = v?.Glosses?.Select(g => g.Text).ToList() ?? new List<string>()
                    };
                }).ToList()
            };
        }
    }
}
