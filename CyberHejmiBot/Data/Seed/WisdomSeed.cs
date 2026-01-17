using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberHejmiBot.Data.Entities.Wisdom;
using CyberHejmiBot.Entities;

namespace CyberHejmiBot.Data.Entities.Seed
{
    public static class WisdomSeed
    {
        public static LocalDbContext SeedWisdom(this LocalDbContext dbContext)
        {
            if (!dbContext.WisdomEntries.Any())
            {
                var wisdoms = new List<WisdomEntry>()
                {
                    new WisdomEntry
                    {
                        AuthorName = "Confucius",
                        Text =
                            "To be fond of learning is near to wisdom; to practice with vigor is near to benevolence; and to be conscious of shame is near to fortitude. He who knows these three things",
                    },
                    new WisdomEntry() { AuthorName = "GREG", Text = "GREG" },
                    new WisdomEntry()
                    {
                        AuthorName = "Josh Billings",
                        Text = "Experience increases our wisdom but doesn't reduce our follies.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Aristophanes",
                        Text = "A man may learn wisdom even from a foe.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Cato the Elder",
                        Text = "It is sometimes the height of wisdom to feign stupidity.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "James Allen",
                        Text =
                            "The more tranquil a man becomes, the greater is his success, his influence, his power for good. Calmness of mind is one of the beautiful jewels of wisdom.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Francis Bacon",
                        Text = "A prudent question is one-half of wisdom.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Bhagavad Gita",
                        Text = "Action should culminate in wisdom.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Alexandre Dumas",
                        Text = "All human wisdom is summed up in two words; wait and hope.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "William Faulkner",
                        Text =
                            "The end of wisdom is to dream high enough to lose the dream in the seeking of it.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "William Arthur Ward",
                        Text =
                            "Committing a great truth to memory is admirable; committing it to life is wisdom.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Aeschylus",
                        Text =
                            "And even in our sleep pain that cannot forget falls drop by drop upon the heart, and in our own despair, against our will, comes wisdom to us by the awful grace of God.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Aeschylus",
                        Text = "Wisdom comes alone through suffering.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Robert Frost",
                        Text = "A poem begins in delight and ends in wisdom.",
                    },
                    new WisdomEntry()
                    {
                        AuthorName = "Henri-Frédéric Amiel",
                        Text =
                            "To know how to grow old is the master work of wisdom, and one of the most difficult chapters in the great art of living.",
                    },
                };

                dbContext.AddRange(wisdoms);
                dbContext.SaveChanges();
            }

            return dbContext;
        }
    }
}
