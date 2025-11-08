using Application.QueryFilters;
using Domain.Entities;
using Domain.Repositories;
using Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class KanjiRepository : IKanjiRepository
    {
        private readonly ApplicationDbContext _context;

        public KanjiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Kanji>> GetAllKanjiAsync(int pageIndex, int pageSize, string? sortBy, CancellationToken cancellationToken = default)
        {
            var query = _context.Kanji
                .Include(k => k.Meanings)
                .AsNoTracking();

            query = KanjiQuerySort.ApplySort(query, sortBy);

            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<Kanji>(items, pageIndex, totalPages);
        }
    }
}