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

            var gradeList = await _context.Kanji
                .Where(k => k.Grade != null)
                .OrderBy(k => k.Grade)
                .ThenBy(k => k.FrequencyRank == null)
                .ThenBy(k => k.FrequencyRank)
                .ToListAsync();

            for (int i = 0; i < gradeList.Count; i++)
                gradeList[i].SortIndex_Grade = i + 1;

            var jlptList = await _context.Kanji
                .Where(k => k.JLPTLevel != null)
                .OrderByDescending(k => k.JLPTLevel)
                .ThenBy(k => k.FrequencyRank == null)
                .ThenBy(k => k.FrequencyRank)
                .ToListAsync();

            for (int i = 0; i < jlptList.Count; i++)
                jlptList[i].SortIndex_JLPT = i + 1;

            await _context.SaveChangesAsync();

            Console.WriteLine("[Importer] Sort indices generated successfully.");
        }
    }
}
