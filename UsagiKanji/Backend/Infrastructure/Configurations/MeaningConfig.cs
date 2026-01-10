using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class MeaningConfig : IEntityTypeConfiguration<Meaning>
    {
        public void Configure(EntityTypeBuilder<Meaning> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Value)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne(m => m.Kanji)
                .WithMany(k => k.Meanings)
                .HasForeignKey(m => m.KanjiId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


