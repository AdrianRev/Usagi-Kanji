using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class VocabularyKanjiCharacterConfig : IEntityTypeConfiguration<VocabularyKanjiCharacter>
    {
        public void Configure(EntityTypeBuilder<VocabularyKanjiCharacter> builder)
        {
            builder.HasKey(kc => kc.Id);

            builder.Property(kc => kc.Reading)
                   .HasMaxLength(100);

            builder.HasOne(kc => kc.Kanji)
                   .WithMany(k => k.VocabularyKanjiCharacters)
                   .HasForeignKey(kc => kc.KanjiId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
