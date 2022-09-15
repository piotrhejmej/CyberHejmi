using CyberHejmiBot.Configuration.Settings;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            foreach(DictionaryEntry e in Environment.GetEnvironmentVariables())
            {
                Console.WriteLine(e.Key + ":" + e.Value);
                Debug.WriteLine(e.Key + ":" + e.Value);
            }
            var connectionString = Environment.GetEnvironmentVariable("Db_ConnectionString");

            Console.WriteLine(connectionString);
            Debug.WriteLine(connectionString);

            optionsBuilder.UseNpgsql(connectionString);
        }

        public void Seed()
        {
            if (!this.TestEntities.Any(r => r.Name == "Test 1"))
                this.Add(new TestEntity { Name = "Test 1", Description = "Desc 1" });


            this.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
