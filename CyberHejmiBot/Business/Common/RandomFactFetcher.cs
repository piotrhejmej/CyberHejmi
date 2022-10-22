using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Common
{
    public record RandomFactResult: WikiEventEntry
    {
        public FactType FactType { get; set; }
    }

    public enum FactType
    {
        Births,
        Deaths,
        Event,
        Holidays
    }

    public interface IRandomFactFetcher
    {
        Task<RandomFactResult> GetRandomFactOfDate(DateTime date, FactType factType);
        Task<RandomFactResult> GetRandomFactOfToday(FactType factType);
    }

    public class RandomFactFetcher : IRandomFactFetcher
    {
        private readonly string WikiApiUriBase = "https://byabbe.se/on-this-day";
        private readonly DateTime Today = DateTime.Now;

        public async Task<RandomFactResult> GetRandomFactOfDate(DateTime date, FactType factType) =>
            await GetRandomFact(date, factType);

        public async Task<RandomFactResult> GetRandomFactOfToday(FactType factType) =>
            await GetRandomFact(Today, factType);

        private async Task<RandomFactResult> GetRandomFact(DateTime date, FactType FactType)
        {
            var randomWikiFact = await GetFact(date, FactType);
            return new RandomFactResult()
            {
                FactType = FactType,
                Description = randomWikiFact?.Description,
                Year = randomWikiFact?.Year
            };
        }

        private async Task<WikiEventEntry?> GetFact(DateTime date, FactType FactType)
        {
            try
            {
                using var client = new HttpClient();
                var uri = $"{WikiApiUriBase}/{date.Month}/{date.Day}/{GetUriTypeSegment(FactType)}";
                var jsonResult = await client.GetStringAsync(uri);
                var result = JsonConvert.DeserializeObject<WikiFactsOfDayResponse>(jsonResult);

                if (result is null)
                    return null;

                var resultCollection = FactType switch
                {
                    FactType.Births => result.Births,
                    FactType.Deaths => result.Deaths,
                    FactType.Event => result.Events,
                    _ => result.Events
                };

                var random = new Random();

                return resultCollection?.ToArray()[random.Next(resultCollection.Count)];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetUriTypeSegment(FactType type) => type switch
        {
            FactType.Event => "events.json",
            FactType.Births => "births.json",
            FactType.Deaths => "deaths.json",
            _ => "events.json"
        };
    }
}
