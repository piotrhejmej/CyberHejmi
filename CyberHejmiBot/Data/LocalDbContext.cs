using CyberHejmiBot.Configuration.Settings;
using CyberHejmiBot.Data.Entities.Facts;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("Db_ConnectionString");

            optionsBuilder.UseNpgsql(connectionString);
        }

        public void Seed()
        {
            if (!this.TestEntities.Any(r => r.Name == "Test 1"))
                this.Add(new TestEntity { Name = "Test 1", Description = "Desc 1" });

            if (!this.WisdomEntries.Any())
            {
                var wisdoms = new List<WisdomEntry>()
                {
                    new WisdomEntry
                    {
                        AuthorName = "Confucius",
                        Text = "To be fond of learning is near to wisdom; to practice with vigor is near to benevolence; and to be conscious of shame is near to fortitude. He who knows these three things"
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "GREG",
                        Text = "GREG"
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Josh Billings",
                        Text = "Experience increases our wisdom but doesn't reduce our follies."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Aristophanes",
                        Text = "A man may learn wisdom even from a foe."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Cato the Elder",
                        Text = "It is sometimes the height of wisdom to feign stupidity."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "James Allen",
                        Text = "The more tranquil a man becomes, the greater is his success, his influence, his power for good. Calmness of mind is one of the beautiful jewels of wisdom."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Francis Bacon",
                        Text = "A prudent question is one-half of wisdom."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Bhagavad Gita",
                        Text = "Action should culminate in wisdom."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Alexandre Dumas",
                        Text = "All human wisdom is summed up in two words; wait and hope."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "William Faulkner",
                        Text = "The end of wisdom is to dream high enough to lose the dream in the seeking of it."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "William Arthur Ward",
                        Text = "Committing a great truth to memory is admirable; committing it to life is wisdom."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Aeschylus",
                        Text = "And even in our sleep pain that cannot forget falls drop by drop upon the heart, and in our own despair, against our will, comes wisdom to us by the awful grace of God."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Aeschylus",
                        Text = "Wisdom comes alone through suffering."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Robert Frost",
                        Text = "A poem begins in delight and ends in wisdom."
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Henri-Frédéric Amiel",
                        Text = "To know how to grow old is the master work of wisdom, and one of the most difficult chapters in the great art of living."
                    }
                };

                this.AddRange(wisdoms);
            }

            this.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
