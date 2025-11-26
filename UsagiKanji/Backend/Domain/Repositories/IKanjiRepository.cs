using Domain.Entities;
using Domain.Utilities;

namespace Domain.Repositories
{
    public interface IKanjiRepository
    {
        Task<PaginatedList<Kanji>> GetAllKanjiAsync(int pageIndex, int pageSize, string? sortBy, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Guid>> GetLearnedKanjiIdsForUserAsync(Guid userId, IReadOnlyList<Guid> kanjiIds, CancellationToken ct = default);
        Task<Kanji?> GetKanjiWithUserDetailsAsync(Guid kanjiId, Guid userId);
        Task<UserKanji?> GetUserKanjiAsync(Guid userId, Guid kanjiId);
        Task<Kanji?> GetNeighborKanjiAsync(Guid kanjiId, string sortBy, bool next);
        Task SaveChangesAsync();
        Task AddAsync(UserKanji userKanji);
        Task UpdateAsync(UserKanji userKanji);
        Task<PaginatedList<UserKanji>> GetDueUserKanjisForUserAsync(Guid userId, DateTime asOfUtcDate, int pageIndex, int pageSize);
        Task<IReadOnlyList<UserKanji>> GetUserKanjisForUserDueAsync(Guid userId, DateTime date);

    }
}
