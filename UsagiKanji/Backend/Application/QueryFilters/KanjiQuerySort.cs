using Domain.Entities;

namespace Application.QueryFilters
{
    public static class KanjiQuerySort
    {
        public static IQueryable<Kanji> ApplySort(IQueryable<Kanji> query, string? sortBy)
        {
            query = sortBy?.ToLower() switch
            {
                "grade" => query.Where(k => k.Grade != null).OrderBy(k => k.Grade).ThenBy(k => k.FrequencyRank),
                "jlptlevel" => query.Where(k => k.JLPTLevel != null).OrderBy(k => k.JLPTLevel).ThenBy(k => k.FrequencyRank),
                "frequency" => query.Where(k => k.FrequencyRank != null).OrderBy(k => k.FrequencyRank),
                "heisig" => query.Where(k => k.HeisigNumber != null).OrderBy(k => k.HeisigNumber),
                "heisig6" => query.Where(k => k.Heisig6Number != null).OrderBy(k => k.Heisig6Number),
                _ => query.Where(k => k.Grade != null).OrderBy(k => k.HeisigNumber)
            };

            return query;
        }
    }
}
