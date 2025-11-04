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

            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var root = JsonSerializer.Deserialize<VocabularyRoot>(json, options);

            if (root?.Words == null || root.Words.Count == 0)
            {
                Console.WriteLine("[Importer] No vocabulary entries found.");
                return;
            }

            foreach (var entry in root.Words)
            {
                var vocab = new Vocabulary
                {
                    JMdictId = entry.Id
                };

                foreach (var kanjiVariant in entry.Kanji)
                {
                    var kanjiForm = new VocabularyKanjiForm
                    {
                        Text = kanjiVariant.Text,
                        Common = kanjiVariant.Common
                    };

                    foreach (var c in kanjiVariant.Text)
                    {
                        var kanjiEntity = await _context.Kanji.FirstOrDefaultAsync(k => k.Character == c.ToString());
                        if (kanjiEntity != null)
                        {
                            kanjiForm.KanjiCharacters.Add(new VocabularyKanjiCharacter
                            {
                                KanjiId = kanjiEntity.Id
                            });
                        }
                    }

                    vocab.KanjiForms.Add(kanjiForm);
                }

                foreach (var kana in entry.Kana)
                {
                    vocab.KanaReadings.Add(new VocabularyKana
                    {
                        Text = kana.Text,
                        Common = kana.Common,
                        AppliesToKanji = string.Join(',', kana.AppliesToKanji)
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
            Console.WriteLine($"[Importer] Imported {root.Words.Count} vocabulary entries.");
        }
    }
}
