using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class VocabularyGlossConfig : IEntityTypeConfiguration<VocabularyGloss>
    {
        public void Configure(EntityTypeBuilder<VocabularyGloss> builder)
        {
            builder.HasKey(g => g.Id);

            builder.Property(g => g.Text)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(g => g.Language)
                   .IsRequired()
                   .HasMaxLength(5);
        }
    }
}
