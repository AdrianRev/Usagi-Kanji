using Domain.Entities;
using Domain.Utilities;

namespace Domain.Repositories
{
    public interface IKanjiRepository
    {
        Task<PaginatedList<Kanji>> GetAllKanjiAsync(int pageIndex, int pageSize, string? sortBy, CancellationToken cancellationToken = default);
    }
}
