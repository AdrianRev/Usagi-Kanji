using Importer.Services;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Importer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("[Importer] Starting import...");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Database.Migrate();
            if (context.Kanji.Any())
            {
                Console.WriteLine("[Importer] Data already imported - skipping.");
                return;
            }
            // Step 1: Import Kanji
            var kanjiImporter = new KanjiImporterService(context);
            await kanjiImporter.ImportKanjiDicJsonAsync(Path.Combine(AppContext.BaseDirectory, "Data", "kanji_dic.json"));

            // Step 2: Import Vocabulary
            var vocabImporter = new VocabularyImporterService(context);
            await vocabImporter.ImportVocabularyJsonAsync(Path.Combine(AppContext.BaseDirectory, "Data", "vocabulary.json"));

            var indicesService = new KanjiIndicesService(context);
            await indicesService.GenerateSortIndicesAsync();



            Console.WriteLine("[Importer] Import completed successfully!");
        }
    }
}
