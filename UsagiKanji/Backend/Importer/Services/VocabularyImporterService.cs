using Domain.Entities;
using Importer.Dtos;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Importer.Services
{
    public class VocabularyImporterService
    {
        private readonly ApplicationDbContext _context;

        public VocabularyImporterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ImportVocabularyJsonAsync(string filePath)
        {
            Console.WriteLine("[Importer] Importing vocabulary JSON...");

            await using var file = File.OpenRead(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var root = await JsonSerializer.DeserializeAsync<VocabularyRoot>(file, options);

            if (root?.Words == null || root.Words.Count == 0)
            {
                Console.WriteLine("[Importer] No vocabulary entries found.");
                return;
            }

            Console.WriteLine($"[Importer] Found {root.Words.Count} vocabulary entries. Starting batched import...");

            var allKanji = await _context.Kanji
                .Select(k => new { k.Id, k.Character })
                .ToDictionaryAsync(k => k.Character, k => k.Id);

            Console.WriteLine($"[Importer] Loaded {allKanji.Count} kanji characters for lookup.");

            const int batchSize = 5000;
            int totalImported = 0;

            for (int i = 0; i < root.Words.Count; i += batchSize)
            {
                var batch = root.Words.Skip(i).Take(batchSize).ToList();

                foreach (var entry in batch)
                {
                    var vocab = new Vocabulary
                    {
                        JMdictId = entry.Id
                    };

                    // Kanji forms
                    foreach (var kanjiVariant in entry.Kanji)
                    {
                        var kanjiForm = new VocabularyKanjiForm
                        {
                            Text = kanjiVariant.Text,
                            Common = kanjiVariant.Common
                        };

                        foreach (var c in kanjiVariant.Text.Distinct())
                        {
                            if (allKanji.TryGetValue(c.ToString(), out var kanjiId))
                            {
                                kanjiForm.KanjiCharacters.Add(new VocabularyKanjiCharacter
                                {
                                    KanjiId = kanjiId
                                });
                            }
                        }

                        vocab.KanjiForms.Add(kanjiForm);
                    }

                    // Kana readings
                    foreach (var kana in entry.Kana)
                    {
                        vocab.KanaReadings.Add(new VocabularyKana
                        {
                            Text = kana.Text,
                            Common = kana.Common,
                            AppliesToKanjiForm = string.Join(',', kana.AppliesToKanjiForm)
                        });
                    }

                    foreach (var sense in entry.Sense)
                    {
                        foreach (var gloss in sense.Gloss)
                        {
                            if (gloss.Lang == "eng")
                            {
                                vocab.Glosses.Add(new VocabularyGloss
                                {
                                    Text = gloss.Text,
                                    Language = gloss.Lang
                                });
                            }
                        }
                    }

                    _context.Vocabulary.Add(vocab);
                }

                await _context.SaveChangesAsync();
                _context.ChangeTracker.Clear();

                totalImported += batch.Count;
                Console.WriteLine($"[Importer] Imported batch: {totalImported}/{root.Words.Count} entries ({(totalImported * 100.0 / root.Words.Count):F1}% complete)");
            }

            Console.WriteLine($"[Importer] Successfully imported {totalImported} vocabulary entries.");
        }
    }
}