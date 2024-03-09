using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberHejmiBot.Business.Common
{
    public interface ITwitchChecker
    {
        Task<bool> IsMrStreamerOnline();
    }

    public class TwitchChecker : ITwitchChecker
    {
        private readonly string TWITCH_CLIENT_ID = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? "";
        private readonly string TWITCH_CLIENT_SECRET = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? "";
        private const string TWITCH_AUTH_API_GRANT_TYPE = "client_credentials";
        private const string TWITCH_AUTH_API_URI = "https://id.twitch.tv/oauth2/token";
        private const string TWITCH_API_URI = "https://api.twitch.tv/helix/streams?user_login=streamkoderka";
        
        public async Task<bool> IsMrStreamerOnline()
        {
            var client = await GetTwitchHttpClient();
            var response = await client.GetAsync(TWITCH_API_URI);
            var responseContent = await response.Content.ReadAsStringAsync();
            var streamResponse = JsonConvert.DeserializeObject<TwitchSteamData>(responseContent);

            return streamResponse?.data.Any() != null;
        }

        private async Task<HttpClient> GetTwitchHttpClient()
        {
            //web request to get auth token from twitch api
            var client = new HttpClient();

            var response = await client.PostAsync(TWITCH_AUTH_API_URI, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", TWITCH_CLIENT_ID },
                { "client_secret", TWITCH_CLIENT_SECRET },
                { "grant_type", TWITCH_AUTH_API_GRANT_TYPE }
            }));
             
            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to get Twitch auth token");

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<TwitchAuthResponse>(responseContent);

            client.BaseAddress = new Uri(TWITCH_API_URI);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authResponse.access_token}");
            client.DefaultRequestHeaders.Add("Client-Id", TWITCH_CLIENT_ID);

            return client;
        }
    }

    internal class TwitchAuthResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }

    internal class TwitchStreamResponse
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string game_id { get; set; }
        public string type { get; set; }
        public string title { get; set; }
        public int viewer_count { get; set; }
        public DateTime started_at { get; set; }
        public string language { get; set; }
        public string thumbnail_url { get; set; }
    }

    internal class TwitchSteamData
    {
        public List<TwitchStreamResponse> data { get; set; }
    }
}
