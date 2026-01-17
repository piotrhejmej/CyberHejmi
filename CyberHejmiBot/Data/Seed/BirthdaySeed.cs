using CyberHejmiBot.Data.Entities.Birthdays;
using CyberHejmiBot.Entities;

namespace CyberHejmiBot.Data.Entities.Seed
{//700750367436046409
    public static class BirthdaySeed
    {
        public static LocalDbContext SeedBirthdays(this LocalDbContext dbContext)
        {
            dbContext.RemoveRange(dbContext.Birthdays);

            var birthdays = new List<Birthday>()
            {
                new Birthday(700750367436046409, "Bartłomiej Gołąbek", new DateTime(1993, 10, 28, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Szymon Niemiec", new DateTime(1993, 1, 14, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Dominik Kownacki", new DateTime(1993, 4, 5, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Ireneusz Kozioł", new DateTime(1993, 4, 23, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Cinek Kłonica", new DateTime(1994, 5, 15, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Karol Klimończyk", new DateTime(1994, 6, 20, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Rafał Jankowski", new DateTime(1993, 11, 25, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Jakub Grzebienoga", new DateTime(1993, 6, 19, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Piotrek Hejmej", new DateTime(1993, 1, 7, 0, 0, 0, DateTimeKind.Utc), "dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa dupa"),
                new Birthday(700750367436046409, "Kamil Kozieł", new DateTime(1994, 11, 21, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Jakub Karamański", new DateTime(1994, 3, 4, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Kamil Gancarczyk", new DateTime(1994, 5, 1, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Bestia z Osieka", new DateTime(1993, 10, 17, 0, 0, 0, DateTimeKind.Utc)),
                new Birthday(700750367436046409, "Kamil Gryzieł", new DateTime(1993, 7, 2, 0, 0, 0, DateTimeKind.Utc)),
            };

            dbContext.AddRange(birthdays);
            dbContext.SaveChanges();

            return dbContext;
        }
    }
}
