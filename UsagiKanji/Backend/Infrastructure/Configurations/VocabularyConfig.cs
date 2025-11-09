using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class VocabularyConfig : IEntityTypeConfiguration<Vocabulary>
    {
        public void Configure(EntityTypeBuilder<Vocabulary> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.JMdictId)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasMany(v => v.KanjiForms)
                   .WithOne(kf => kf.Vocabulary)
                   .HasForeignKey(kf => kf.VocabularyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.KanaReadings)
                   .WithOne(k => k.Vocabulary)
                   .HasForeignKey(k => k.VocabularyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.Glosses)
                   .WithOne(g => g.Vocabulary)
                   .HasForeignKey(g => g.VocabularyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
