using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class VocabularyKanaConfig : IEntityTypeConfiguration<VocabularyKana>
    {
        public void Configure(EntityTypeBuilder<VocabularyKana> builder)
        {
            builder.HasKey(k => k.Id);

            builder.Property(k => k.Text)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(k => k.AppliesToKanjiForm)
                   .IsRequired()
                   .HasMaxLength(100);
        }
    }
}
