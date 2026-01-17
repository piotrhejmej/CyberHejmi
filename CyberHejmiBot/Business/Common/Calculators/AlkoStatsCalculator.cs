using CyberHejmiBot.Data.Entities.Alcohol;
using Discord;

namespace CyberHejmiBot.Business.Common.Calculators
{
    public record AlkoStatsResult(
        int UniqueDays,
        double PercentageOfDays,
        double TotalPureAlcoholMl,
        double RedStageBottles
    );

    public interface IAlkoStatsCalculator
    {
        AlkoStatsResult Calculate(IEnumerable<AlkoStat> logs, int year);
        Embed BuildEmbed(AlkoStatsResult stats, int year, string title, string description);
    }

    public class AlkoStatsCalculator : IAlkoStatsCalculator
    {
        private const double RedStagePercentage = 32.5;
        private const double RedStageVolumeMl = 700.0;
        private const double RedStagePureAlcoholMl =
            RedStageVolumeMl * (RedStagePercentage / 100.0);

        public AlkoStatsResult Calculate(IEnumerable<AlkoStat> logs, int year)
        {
            var uniqueDays = logs.GroupBy(x => x.Date.Date).Count();

            var isCurrentYear = DateTime.UtcNow.Year == year;
            var endCalculationDate = isCurrentYear
                ? DateTime.UtcNow.Date
                : new DateTime(year, 12, 31);
            var startCalculationDate = new DateTime(year, 1, 1);

            var totalDaysInPeriod = (endCalculationDate - startCalculationDate).Days + 1;

            if (totalDaysInPeriod <= 0)
                totalDaysInPeriod = 1;

            var percentageOfDays = (double)uniqueDays / totalDaysInPeriod * 100;

            var totalPureAlcoholMl = logs.Sum(x =>
                (x.AmountMl ?? 0) * ((x.Percentage ?? 0) / 100.0)
            );
            var redStageBottles = totalPureAlcoholMl / RedStagePureAlcoholMl;

            return new AlkoStatsResult(
                uniqueDays,
                percentageOfDays,
                totalPureAlcoholMl,
                redStageBottles
            );
        }

        public Embed BuildEmbed(AlkoStatsResult stats, int year, string title, string description)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .AddField("Days Logged", $"{stats.UniqueDays} days", true)
                .AddField("Percentage of Days", $"{stats.PercentageOfDays:F2}%", true);

            if (stats.TotalPureAlcoholMl > 0)
            {
                embedBuilder.AddField(
                    "Total Pure Alcohol",
                    $"{stats.TotalPureAlcoholMl:F0} ml",
                    true
                );
                embedBuilder.AddField(
                    "Red Stag Bottles (equiv)",
                    $"{stats.RedStageBottles:F1} 🍾",
                    true
                );
            }

            return embedBuilder.Build();
        }
    }
}
