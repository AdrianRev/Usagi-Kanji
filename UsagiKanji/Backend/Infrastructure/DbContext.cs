using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Kanji> Kanji { get; set; }
        public DbSet<Vocabulary> Vocabulary { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserKanji> UserKanji { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
