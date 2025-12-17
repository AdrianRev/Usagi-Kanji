using Infrastructure;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Importer.Services
{
    public class KanjiIndicesService
    {
        private readonly ApplicationDbContext _context;

        public KanjiIndicesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GenerateSortIndicesAsync()
        {
            Console.WriteLine("[Importer] Generating sort index values...");

            const int batchSize = 2000;

            int gradeIndex = 1;
            int skip = 0;
            while (true)
            {
                var batch = await _context.Kanji
                    .Where(k => k.Grade != null)
                    .OrderBy(k => k.Grade)
                    .ThenBy(k => k.FrequencyRank == null)
                    .ThenBy(k => k.FrequencyRank)
                    .Skip(skip)
                    .Take(batchSize)
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var kanji in batch)
                {
                    kanji.SortIndex_Grade = gradeIndex++;
                }

                await _context.SaveChangesAsync();
                skip += batchSize;
            }

            int jlptIndex = 1;
            skip = 0;
            while (true)
            {
                var batch = await _context.Kanji
                    .Where(k => k.JLPTLevel != null)
                    .OrderByDescending(k => k.JLPTLevel)
                    .ThenBy(k => k.FrequencyRank == null)
                    .ThenBy(k => k.FrequencyRank)
                    .Skip(skip)
                    .Take(batchSize)
                    .ToListAsync();

                if (!batch.Any()) break;

                foreach (var kanji in batch)
                {
                    kanji.SortIndex_JLPT = jlptIndex++;
                }

                await _context.SaveChangesAsync();
                skip += batchSize;
            }

            Console.WriteLine("[Importer] Sort indices generated successfully.");
        }
    }
}
