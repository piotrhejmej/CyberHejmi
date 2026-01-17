using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Common
{
    public interface ITwitchChecker
    {
        Task<CheckerResult> IsMrStreamerOnline();
    }

    public class CheckerResult
    {
        public bool IsSuccesfull { get; set; }
        public string? Error { get; set; }
        public bool Result { get; set; }
    }

    public class TwitchChecker : ITwitchChecker
    {
        private readonly string TWITCH_CLIENT_ID = Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID") ?? "";
        private readonly string TWITCH_CLIENT_SECRET = Environment.GetEnvironmentVariable("TWITCH_CLIENT_SECRET") ?? "";
        private const string TWITCH_AUTH_API_GRANT_TYPE = "client_credentials";
        private const string TWITCH_AUTH_API_URI = "https://id.twitch.tv/oauth2/token";
        private const string TWITCH_API_URI = "https://api.twitch.tv/helix/streams?user_login=StreamKoderka";

        public async Task<CheckerResult> IsMrStreamerOnline()
        {
            var clientResult = await GetAuthorizedTwitchHttpClient();

            if (!clientResult.isSuccessfull || clientResult.httpClient is null)
                return new CheckerResult
                {
                    IsSuccesfull = false,
                    Error = $"Code: {clientResult.statusCode}"
                };

            var response = await clientResult.httpClient.GetAsync(TWITCH_API_URI);

            if (!response.IsSuccessStatusCode)
            {
                return new CheckerResult
                {
                    IsSuccesfull = false,
                    Error = $"{response.StatusCode}"
                };
            };

            var responseContent = await response.Content.ReadAsStringAsync();
            var streamResponse = JsonConvert.DeserializeObject<TwitchSteamData>(responseContent);

            clientResult.httpClient.Dispose();

            return new CheckerResult
            {
                IsSuccesfull = true,
                Result = streamResponse?.data?.Any() == true
            };
        }

        private async Task<(bool isSuccessfull, HttpClient? httpClient, HttpStatusCode? statusCode)> GetAuthorizedTwitchHttpClient()
        {
            var client = new HttpClient();

            var response = await client.PostAsync($"{TWITCH_AUTH_API_URI}?client_id={TWITCH_CLIENT_ID}&client_secret={TWITCH_CLIENT_SECRET}&grant_type={TWITCH_AUTH_API_GRANT_TYPE}", null);

            if (!response.IsSuccessStatusCode)
            {
                return (false, null, response.StatusCode);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<TwitchAuthResponse>(responseContent);

            client.Dispose();
            client = new HttpClient();

            if (authResponse?.access_token is null)
                return (false, null, null);

            client.BaseAddress = new Uri(TWITCH_API_URI);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authResponse.access_token}");
            client.DefaultRequestHeaders.Add("Client-Id", TWITCH_CLIENT_ID);

            return (true, client, null);
        }
    }

    internal class TwitchAuthResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
    }

    internal class TwitchStreamResponse
    {
        public string? id { get; set; }
        public string? user_id { get; set; }
        public string? user_name { get; set; }
        public string? game_id { get; set; }
        public string? type { get; set; }
        public string? title { get; set; }
        public int viewer_count { get; set; }
        public DateTime started_at { get; set; }
        public string? language { get; set; }
        public string? thumbnail_url { get; set; }
    }

    internal class TwitchSteamData
    {
        public List<TwitchStreamResponse>? data { get; set; }
    }
}
