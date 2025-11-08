using Domain.Entities;
using Domain.Enums;
using Importer.Dtos;
using Infrastructure;
using System.Text.Json;

namespace Importer.Services
{
    public class KanjiImporterService
    {
        private readonly ApplicationDbContext _context;

        public KanjiImporterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ImportKanjiDicJsonAsync(string filePath)
        {
            Console.WriteLine("[Importer] Importing kanji_dic.json...");

            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var root = JsonSerializer.Deserialize<KanjiDicRoot>(json, options);

            if (root?.Characters == null)
            {
                Console.WriteLine("[Importer] No characters found in JSON.");
                return;
            }

            foreach (var entry in root.Characters)
            {
                var kanji = new Kanji
                {
                    Character = entry.Literal,
                    StrokeCount = entry.Misc?.StrokeCounts?.FirstOrDefault(),
                    Grade = entry.Misc?.Grade,
                    JLPTLevel = entry.Misc?.JlptLevel,
                    FrequencyRank = entry.Misc?.Frequency,
                    HeisigNumber = entry.DictionaryReferences?.FirstOrDefault(d => d.Type == "heisig")?.ValueAsInt(),
                    Heisig6Number = entry.DictionaryReferences?.FirstOrDefault(d => d.Type == "heisig6")?.ValueAsInt()
                };

                if (entry.ReadingMeaning?.Groups != null)
                {
                    foreach (var group in entry.ReadingMeaning.Groups)
                    {
                        foreach (var reading in group.Readings)
                        {
                            ReadingType? type = reading.Type switch
                            {
                                "ja_on" => ReadingType.Onyomi,
                                "ja_kun" => ReadingType.Kunyomi,
                                _ => null
                            };

                            if (type.HasValue)
                            {
                                kanji.Readings.Add(new Reading
                                {
                                    Type = type.Value,
                                    Value = reading.Value
                                });
                            }
                        }

                        // Add meanings
                        bool isFirst = true;
                        foreach (var meaning in group.Meanings.Where(m => m.Lang == "en"))
                        {
                            kanji.Meanings.Add(new Meaning
                            {
                                Language = meaning.Lang,
                                Value = meaning.Value,
                                IsPrimary = isFirst
                            });
                            isFirst = false;
                        }
                    }
                }

                // Add Nanori readings
                if (entry.Nanori != null)
                {
                    foreach (var nanori in entry.Nanori)
                    {
                        kanji.Readings.Add(new Reading
                        {
                            Type = ReadingType.Nanori,
                            Value = nanori
                        });
                    }
                }

                _context.Kanji.Add(kanji);
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"[Importer] Imported {root.Characters.Count} kanji entries.");
        }

    }

    internal static class KanjiDicExtensions
    {
        public static int? ValueAsInt(this DictionaryReference reference)
        {
            if (int.TryParse(reference?.Value, out int result))
                return result;
            return null;
        }
    }
}
