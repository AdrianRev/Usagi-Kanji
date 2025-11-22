using Application.Dtos;
using Domain.Utilities;
using FluentResults;

namespace Application.Interfaces
{
    public interface IKanjiService
    {
        Task<Result<PaginatedList<KanjiListItemDto>>> GetAllKanjiAsync(KanjiListParams parameters, Guid userId, CancellationToken cancellationToken = default);
        Task<KanjiDetailsDto?> GetKanjiByIdAsync(Guid kanjiId, Guid userId);
        Task<Result> UpdateOrAddUserKanjiAsync(Guid kanjiId, Guid userId, UpdateOrAddUserKanjiDto dto);
        Task<KanjiDetailsDto?> GetNeighborKanjiAsync(Guid kanjiId, Guid userId, string sortBy, bool next);
    }
}
