using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class VocabularyKanjiFormConfig : IEntityTypeConfiguration<VocabularyKanjiForm>
    {
        public void Configure(EntityTypeBuilder<VocabularyKanjiForm> builder)
        {
            builder.HasKey(kf => kf.Id);

            builder.Property(kf => kf.Text)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.HasMany(kf => kf.KanjiCharacters)
                    .WithOne(kc => kc.KanjiForm)
                    .HasForeignKey(kc => kc.VocabularyKanjiFormId)
                    .OnDelete(DeleteBehavior.Cascade);

            builder.Property(kf => kf.Common)
                   .IsRequired();

            builder.HasOne(kf => kf.Vocabulary)
                   .WithMany(v => v.KanjiForms)
                   .HasForeignKey(kf => kf.VocabularyId);
        }
    }
}
