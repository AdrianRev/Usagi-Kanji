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

        public async Task<IReadOnlyList<Guid>> GetLearnedKanjiIdsForUserAsync(Guid userId, IReadOnlyList<Guid> kanjiIds, CancellationToken ct = default)
        {
            return await _context.UserKanjis
                .Where(uk => uk.UserId == userId && kanjiIds.Contains(uk.KanjiId))
                .Select(uk => uk.KanjiId)
                .ToListAsync(ct);
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

        public async Task<Kanji?> GetNeighborKanjiAsync(Guid kanjiId, string sortBy, bool next)
        {
            var current = await _context.Kanji.FirstOrDefaultAsync(k => k.Id == kanjiId);
            if (current == null) return null;

            IQueryable<Kanji> query = _context.Kanji.AsNoTracking();

            switch (sortBy.ToLower())
            {
                case "grade":
                    query = next
                        ? query.Where(k => k.SortIndex_Grade > current.SortIndex_Grade).OrderBy(k => k.SortIndex_Grade)
                        : query.Where(k => k.SortIndex_Grade < current.SortIndex_Grade).OrderByDescending(k => k.SortIndex_Grade);
                    break;
                case "jlptlevel":
                    query = next
                        ? query.Where(k => k.SortIndex_JLPT > current.SortIndex_JLPT).OrderBy(k => k.SortIndex_JLPT)
                        : query.Where(k => k.SortIndex_JLPT < current.SortIndex_JLPT).OrderByDescending(k => k.SortIndex_JLPT);
                    break;
                case "frequency":
                    query = next
                        ? query.Where(k => k.FrequencyRank > current.FrequencyRank).OrderBy(k => k.FrequencyRank)
                        : query.Where(k => k.FrequencyRank < current.FrequencyRank).OrderByDescending(k => k.FrequencyRank);
                    break;
                case "heisig":
                    query = next
                        ? query.Where(k => k.HeisigNumber > current.HeisigNumber).OrderBy(k => k.HeisigNumber)
                        : query.Where(k => k.HeisigNumber < current.HeisigNumber).OrderByDescending(k => k.HeisigNumber);
                    break;
                case "heisig6":
                default:
                    query = next
                        ? query.Where(k => k.Heisig6Number > current.Heisig6Number).OrderBy(k => k.Heisig6Number)
                        : query.Where(k => k.Heisig6Number < current.Heisig6Number).OrderByDescending(k => k.Heisig6Number);
                    break;
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<UserKanji>> GetUserKanjisForUserDueAsync(Guid userId, DateTime date)
        {
            return await _context.UserKanjis
                .Include(uk => uk.Kanji)
                .Where(uk => uk.UserId == userId && uk.NextReviewDate <= date)
                .ToListAsync();
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