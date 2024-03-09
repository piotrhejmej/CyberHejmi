using CyberHejmiBot.Configuration.Settings;
using CyberHejmiBot.Data.Entities.Birthdays;
using CyberHejmiBot.Data.Entities.Facts;
using CyberHejmiBot.Data.Entities.JobRelated;
using CyberHejmiBot.Data.Entities.Seed;
using CyberHejmiBot.Data.Entities.Wisdom;
using CyberHejmiBot.Entities.Test;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Entities
{
    public class LocalDbContext : DbContext
    { 
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();
        public DbSet<WisdomEntry> WisdomEntries => Set<WisdomEntry>();
        public DbSet<FactsSubscription> FactsSubscriptions => Set<FactsSubscription>();
        public DbSet<Birthday> Birthdays => Set<Birthday>();
        public DbSet<MrStreamerCheckerLogs> MrStreamerCheckerLogs => Set<MrStreamerCheckerLogs>();
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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
