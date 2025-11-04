using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class KanjiConfig : IEntityTypeConfiguration<Kanji>
    {
        public void Configure(EntityTypeBuilder<Kanji> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.Character)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(true);


            builder.Property(k => k.StrokeCount)
                .IsRequired(false);

            builder.Property(k => k.Grade)
                .IsRequired(false);

            builder.Property(k => k.JLPTLevel)
                .IsRequired(false);

            builder.Property(k => k.FrequencyRank)
                .IsRequired(false);

            builder.Property(k => k.HeisigNumber)
                .IsRequired(false);

            builder.Property(k => k.Heisig6Number)
                .IsRequired(false);

            builder.HasMany(k => k.Readings)
                .WithOne(r => r.Kanji)
                .HasForeignKey(r => r.KanjiId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(k => k.Readings).AutoInclude();

            builder.HasMany(k => k.Meanings)
                .WithOne(m => m.Kanji)
                .HasForeignKey(m => m.KanjiId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(k => k.VocabularyKanjiCharacters)
                .WithOne(vkc => vkc.Kanji)
                .HasForeignKey(vkc => vkc.KanjiId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(k => k.Meanings).AutoInclude();
        }
    }
}
