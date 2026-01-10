using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Configurations
{
    internal class ReadingConfig : IEntityTypeConfiguration<Reading>
    {
        public void Configure(EntityTypeBuilder<Reading> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Type)
                .IsRequired()
                .HasMaxLength(20)
                .HasConversion(new EnumToStringConverter<ReadingType>());

            builder.Property(r => r.Value)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(r => r.Kanji)
                .WithMany(k => k.Readings)
                .HasForeignKey(r => r.KanjiId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

