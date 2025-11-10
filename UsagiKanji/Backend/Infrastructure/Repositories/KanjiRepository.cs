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

        public async Task<Kanji?> GetKanjiWithUserDetailsAsync(Guid kanjiId, Guid userId)
        {
            return await _context.Kanji
                .Include(k => k.Readings)
                .Include(k => k.Meanings)
                .Include(k => k.UserKanjis.Where(uk => uk.UserId == userId))
                .Include(k => k.VocabularyKanjiCharacters)
                    .ThenInclude(vkc => vkc.KanjiForm)
                        .ThenInclude(vf => vf.Vocabulary)
                            .ThenInclude(v => v.Glosses)
                .Include(k => k.VocabularyKanjiCharacters)
                    .ThenInclude(vkc => vkc.KanjiForm)
                        .ThenInclude(vf => vf.Vocabulary)
                            .ThenInclude(v => v.KanaReadings)
                .FirstOrDefaultAsync(k => k.Id == kanjiId);
        }
        public async Task<UserKanji?> GetUserKanjiAsync(Guid userId, Guid kanjiId)
        {
            return await _context.UserKanjis
                .FirstOrDefaultAsync(uk => uk.UserId == userId && uk.KanjiId == kanjiId);
        }

        public async Task AddAsync(UserKanji userKanji)
        {
            await _context.UserKanjis.AddAsync(userKanji);
        }

        public async Task UpdateAsync(UserKanji userKanji)
        {
            _context.UserKanjis.Update(userKanji);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}