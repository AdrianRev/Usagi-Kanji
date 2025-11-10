using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class UserKanjiConfig : IEntityTypeConfiguration<UserKanji>
    {
        public void Configure(EntityTypeBuilder<UserKanji> builder)
        {
            builder.HasKey(uk => uk.Id);

            builder.HasOne(uk => uk.User)
                   .WithMany(u => u.UserKanjis)
                   .HasForeignKey(uk => uk.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uk => uk.Kanji)
                   .WithMany(k => k.UserKanjis)
                   .HasForeignKey(uk => uk.KanjiId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(uk => uk.Notes)
                   .HasMaxLength(1000);

            builder.Property(uk => uk.Keyword)
                   .HasMaxLength(200);

            builder.Property(uk => uk.EaseFactor)
                   .HasDefaultValue(2.5);

            builder.Property(uk => uk.Interval)
                   .HasDefaultValue(0);
        }
    }
}
