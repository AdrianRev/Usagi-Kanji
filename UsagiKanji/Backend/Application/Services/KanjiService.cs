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

        public async Task<Result<PaginatedList<KanjiListItemDto>>> GetAllKanjiAsync(KanjiListParams parameters, CancellationToken cancellationToken = default)
        {
            var kanjiPage = await _kanjiRepository.GetAllKanjiAsync(parameters.PageIndex, parameters.PageSize, parameters.SortBy, cancellationToken);

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
    }
}
