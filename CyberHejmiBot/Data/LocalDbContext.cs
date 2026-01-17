using CyberHejmiBot.Data.Entities.Alcohol;
using CyberHejmiBot.Data.Entities.Birthdays;
using CyberHejmiBot.Data.Entities.Facts;
using CyberHejmiBot.Data.Entities.JobRelated;
using CyberHejmiBot.Data.Entities.Karma;
using CyberHejmiBot.Data.Entities.Seed;
using CyberHejmiBot.Data.Entities.Wisdom;
using CyberHejmiBot.Entities.Test;
using Microsoft.EntityFrameworkCore;

namespace CyberHejmiBot.Entities
{
    public class LocalDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
        public DbSet<WisdomEntry> WisdomEntries => Set<WisdomEntry>();
        public DbSet<FactsSubscription> FactsSubscriptions => Set<FactsSubscription>();
        public DbSet<Birthday> Birthdays => Set<Birthday>();
        public DbSet<UserKarma> UserKarma => Set<UserKarma>();
        public DbSet<MrStreamerCheckerLogs> MrStreamerCheckerLogs => Set<MrStreamerCheckerLogs>();
        public DbSet<AlkoStat> AlkoStats => Set<AlkoStat>();

        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
        {
        }

        public LocalDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            var connectionString = Environment.GetEnvironmentVariable("Db_ConnectionString");

            if (connectionString == null)
                throw new ArgumentException("DB connection string is missing");

            optionsBuilder.UseNpgsql(connectionString);
        }

        public void Seed()
        {
            this.SeedWisdom()
                .SeedBirthdays();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
