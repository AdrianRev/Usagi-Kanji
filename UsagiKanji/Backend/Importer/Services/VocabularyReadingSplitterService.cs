using Domain.Entities;
using Domain.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Importer.Services
{
    public class VocabularyReadingSplitterService
    {
        private readonly ApplicationDbContext _context;
        private Dictionary<string, List<string>> _kanjiReadings = new();

        public VocabularyReadingSplitterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AssignReadingsAsync(bool concatenateDuplicates = false)
        {
            Console.WriteLine("[Splitter] Building kanji readings dictionary...");
            await BuildKanjiDictionaryAsync();

            Console.WriteLine($"[Splitter] Loaded {_kanjiReadings.Count} kanji entries.");

            var vocabList = await _context.Vocabulary
                .Include(v => v.KanjiForms)
                    .ThenInclude(f => f.KanjiCharacters)
                        .ThenInclude(kc => kc.Kanji)
                            .ThenInclude(k => k.Readings)
                .Include(v => v.KanaReadings)
                .ToListAsync();

            int processed = 0;
            int assigned = 0;
            int failures = 0;

            foreach (var vocab in vocabList)
            {
                var kanaReadings = vocab.KanaReadings ?? Enumerable.Empty<VocabularyKana>();
                var forms = vocab.KanjiForms ?? Enumerable.Empty<VocabularyKanjiForm>();

                foreach (var form in forms)
                {
                    if (string.IsNullOrWhiteSpace(form?.Text))
                        continue;

                    var applicableKana = GetApplicableKanaReadings(kanaReadings, form.Text);

                    if (!applicableKana.Any())
                        continue;

                    bool formMapped = false;
                    foreach (var kana in applicableKana)
                    {
                        var kanaText = NormalizeToHiragana(kana.Text);
                        var tokens = NormalizeAppliesTokens(kana.AppliesToKanji);

                        var chars = form.Text.ToCharArray().Select(c => c.ToString()).ToArray();

                        var kanjiEntitiesAtPos = new VocabularyKanjiCharacter?[chars.Length];
                        if (form.KanjiCharacters != null && form.KanjiCharacters.Count == chars.Length)
                        {
                            for (int i = 0; i < chars.Length; i++)
                                kanjiEntitiesAtPos[i] = form.KanjiCharacters.ElementAt(i);
                        }
                        else
                        {
                            for (int i = 0; i < chars.Length; i++)
                            {
                                var ch = chars[i];
                                var found = form.KanjiCharacters?.FirstOrDefault(kc => kc.Kanji != null && kc.Kanji.Character == ch);
                                kanjiEntitiesAtPos[i] = found;
                            }
                        }

                        var mapping = new string?[chars.Length];
                        var success = TrySplitRecursive(chars, kanjiEntitiesAtPos, 0, kanaText, mapping);

                        if (success)
                        {
                            for (int i = 0; i < chars.Length; i++)
                            {
                                var vkChar = kanjiEntitiesAtPos[i];
                                if (vkChar == null)
                                    continue;

                                if (tokens != null && !TokensApplyToPosition(tokens, form.Text, i))
                                    continue;

                                var piece = mapping[i];
                                if (string.IsNullOrWhiteSpace(piece))
                                    continue;

                                if (concatenateDuplicates && !string.IsNullOrWhiteSpace(vkChar.Reading) && vkChar.Reading != piece)
                                    vkChar.Reading = $"{vkChar.Reading}, {piece}";
                                else
                                    vkChar.Reading = piece;

                                assigned++;
                            }

                            formMapped = true;
                            break;
                        }
                        else
                        {
                            failures++;
                        }
                    }

                    if (formMapped)
                        processed++;
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"[Splitter] Done. forms processed: {processed}, assigned pieces: {assigned}, failed attempts: {failures}");
        }

        private async Task BuildKanjiDictionaryAsync()
        {
            _kanjiReadings = new Dictionary<string, List<string>>();

            var kanjis = await _context.Kanji
                .Include(k => k.Readings)
                .AsNoTracking()
                .ToListAsync();

            foreach (var k in kanjis)
            {
                if (string.IsNullOrEmpty(k.Character))
                    continue;

                var list = new List<string>();

                if (k.Readings != null)
                {
                    foreach (var r in k.Readings)
                    {
                        if (r.Type == ReadingType.Nanori)
                            continue;

                        var v = r.Value ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(v))
                            continue;

                        var normalized = NormalizeToHiragana(v);
                        if (!list.Contains(normalized))
                            list.Add(normalized);
                    }
                }

                list = list.OrderByDescending(s => s.Length).ToList();

                if (list.Any())
                    _kanjiReadings[k.Character] = list;
            }
        }

        private List<VocabularyKana> GetApplicableKanaReadings(IEnumerable<VocabularyKana> kanaReadings, string formText)
        {
            var list = new List<VocabularyKana>();

            foreach (var k in kanaReadings)
            {
                if (k == null) continue;
                var tokensRaw = k.AppliesToKanji?.Trim();

                if (string.IsNullOrWhiteSpace(tokensRaw) || tokensRaw == "*")
                {
                    list.Add(k);
                    continue;
                }

                var tokens = tokensRaw.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();

                if (tokens.Any(t => t == formText))
                {
                    list.Add(k);
                    continue;
                }
                if (tokens.Any(t => t.Length == 1 && formText.Contains(t)))
                {
                    list.Add(k);
                    continue;
                }
            }

            return list;
        }

        private string[]? NormalizeAppliesTokens(string? applies)
        {
            if (string.IsNullOrWhiteSpace(applies) || applies.Trim() == "*")
                return null;

            var tokens = applies.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray();
            return tokens.Length == 0 ? null : tokens;
        }

        private bool TokensApplyToPosition(string[] tokens, string formText, int position)
        {
            if (tokens == null || tokens.Length == 0)
                return true;

            if (tokens.Any(t => t == formText))
                return true;

            var ch = formText[position].ToString();
            return tokens.Any(t => t.Length == 1 && t == ch);
        }

        private bool TrySplitRecursive(string[] chars, VocabularyKanjiCharacter?[] kanjiEntitiesAtPos, int pos, string kanaRemaining, string?[] mapping)
        {
            if (pos >= chars.Length)
            {
                return string.IsNullOrEmpty(kanaRemaining) || kanaRemaining.All(c => c == 'ー');
            }

            if (kanaRemaining == null)
                return false;

            var currentChar = chars[pos];
            var vkChar = kanjiEntitiesAtPos[pos];
            var isKanji = vkChar != null && !string.IsNullOrEmpty(vkChar.Kanji?.Character);

            if (currentChar == "々")
            {
                if (pos == 0 || mapping[pos - 1] == null)
                    return false;

                mapping[pos] = mapping[pos - 1];
                return TrySplitRecursive(chars, kanjiEntitiesAtPos, pos + 1, kanaRemaining, mapping);
            }

            if (!isKanji)
            {
                var charH = NormalizeToHiragana(currentChar);

                if (kanaRemaining.StartsWith(charH))
                {
                    mapping[pos] = charH;
                    return TrySplitRecursive(chars, kanjiEntitiesAtPos, pos + 1, kanaRemaining.Substring(charH.Length), mapping);
                }

                if (!string.IsNullOrEmpty(kanaRemaining) && kanaRemaining[0].ToString() == currentChar)
                {
                    mapping[pos] = currentChar;
                    return TrySplitRecursive(chars, kanjiEntitiesAtPos, pos + 1, kanaRemaining.Substring(1), mapping);
                }
                return false;
            }

            if (!_kanjiReadings.TryGetValue(currentChar, out var candList) || candList == null || candList.Count == 0)
            {
                return false;
            }

            var candidateVariants = new List<string>();
            foreach (var baseReading in candList)
            {
                candidateVariants.Add(baseReading);
                candidateVariants.AddRange(GenerateHardenedVariants(baseReading));
            }

            candidateVariants = candidateVariants.Distinct().OrderByDescending(s => s.Length).ToList();

            foreach (var cand in candidateVariants)
            {
                if (string.IsNullOrEmpty(cand))
                    continue;

                if (!kanaRemaining.StartsWith(cand))
                    continue;

                mapping[pos] = cand;
                var newRemaining = kanaRemaining.Substring(cand.Length);

                if (TrySplitRecursive(chars, kanjiEntitiesAtPos, pos + 1, newRemaining, mapping))
                    return true;

                mapping[pos] = null;
            }

            for (int fallbackLen = Math.Min(2, kanaRemaining.Length); fallbackLen >= 1; fallbackLen--)
            {
                var piece = kanaRemaining.Substring(0, fallbackLen);
                mapping[pos] = piece;
                var remainder = kanaRemaining.Substring(fallbackLen);
                if (TrySplitRecursive(chars, kanjiEntitiesAtPos, pos + 1, remainder, mapping))
                    return true;
                mapping[pos] = null;
            }

            return false;
        }

        private static string NormalizeToHiragana(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (ch >= '\u30A1' && ch <= '\u30F4')
                {
                    sb.Append((char)(ch - 0x60));
                }
                else
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }

        private static IEnumerable<string> GenerateHardenedVariants(string baseReading)
        {
            if (string.IsNullOrEmpty(baseReading)) yield break;

            var initialMap = new Dictionary<string, string>
            {
                {"か","が"}, {"き","ぎ"}, {"く","ぐ"}, {"け","げ"}, {"こ","ご"},
                {"さ","ざ"}, {"し","じ"}, {"す","ず"}, {"せ","ぜ"}, {"そ","ぞ"},
                {"た","だ"}, {"ち","ぢ"}, {"つ","づ"}, {"て","で"}, {"と","ど"},
                {"は","ば"}, {"ひ","び"}, {"ふ","ぶ"}, {"へ","べ"}, {"ほ","ぼ"},
                {"は-p","ぱ"}, {"ひ-p","ぴ"}, {"ふ-p","ぷ"}, {"へ-p","ぺ"}, {"ほ-p","ぽ"}
            };

            var firstKana = baseReading.Length >= 1 ? baseReading.Substring(0, 1) : "";
            if (initialMap.ContainsKey(firstKana))
            {
                var voiced = initialMap[firstKana];
                yield return voiced + baseReading.Substring(1);
            }

            if (new[] { "は", "ひ", "ふ", "へ", "ほ" }.Contains(firstKana))
            {
                var pmap = new Dictionary<string, string> { { "は", "ぱ" }, { "ひ", "ぴ" }, { "ふ", "ぷ" }, { "へ", "ぺ" }, { "ほ", "ぽ" } };
                if (pmap.TryGetValue(firstKana, out var pvar))
                    yield return pvar + baseReading.Substring(1);
            }

            if (baseReading.EndsWith("つ") || baseReading.EndsWith("く"))
            {
                yield return baseReading.Substring(0, baseReading.Length - 1) + "っ";
            }
        }
    }
}