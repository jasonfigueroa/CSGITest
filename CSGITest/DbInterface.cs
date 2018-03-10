using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSGITest
{
    class DbInterface
    {
        public static bool IsValidUser(string username, string password)
        {
            UserAuth userAuth = new UserAuth(username, password);

            JWT jwt = Auth(userAuth).GetAwaiter().GetResult();

            if (jwt.access_token == null)
            {
                return false;
            }

            return true;
        }

        static async Task<JWT> Auth(UserAuth userAuth)
        {
            HttpClient client = new HttpClient();

            string url = "http://localhost:5000/auth";

            string serializedUserAuth = JsonConvert.SerializeObject(userAuth);
            StringContent authStringContent = new StringContent(serializedUserAuth, Encoding.UTF8, "application/json");

            HttpResponseMessage authResponseMessage = await client.PostAsync(url, authStringContent);
            string authResponseString = await authResponseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<JWT>(authResponseString);
        }

        public static string GetSteamId(UserAuth userAuth)
        {
            return GetSteamIdAsync(userAuth).GetAwaiter().GetResult();
        }

        static async Task<string> GetSteamIdAsync(UserAuth userAuth)
        {
            HttpClient client = new HttpClient();

            JWT jwt = Auth(userAuth).GetAwaiter().GetResult();

            string url = "http://localhost:5000/user/steamid";

            client.DefaultRequestHeaders.Add("Authorization", $"JWT {jwt.access_token}");

            var responseString = await client.GetStringAsync(url);
            UserSteamId steamId = JsonConvert.DeserializeObject<UserSteamId>(responseString);
            
            return steamId.steam_id;
        }

        public static void SaveToDb(UserAuth userAuth, Match match, MatchStats matchStats)
        {
            JWT jwt;
            int matchId;

            jwt = Auth(userAuth).GetAwaiter().GetResult();

            matchId = PostMatchAsync(userAuth, jwt, match).GetAwaiter().GetResult();

            matchStats.match_id = matchId;

            PostMatchStatsAsync(userAuth, jwt, matchStats).GetAwaiter().GetResult();
        }

        static async Task PostMatchStatsAsync(UserAuth userAuth, JWT jwt, MatchStats matchStats)
        {
            HttpClient client = new HttpClient();

            string url = "http://localhost:5000/matchstats";

            client.DefaultRequestHeaders.Add("Authorization", $"JWT {jwt.access_token}");

            string serializedMatchStats = JsonConvert.SerializeObject(matchStats);

            StringContent matchStatsStringContent = new StringContent(serializedMatchStats, Encoding.UTF8, "application/json");

            HttpResponseMessage matchStatsResponseMessage = await client.PostAsync(url, matchStatsStringContent);
        }

        static async Task<int> PostMatchAsync(UserAuth userAuth, JWT jwt, Match match)
        {
            HttpClient client = new HttpClient();

            string url = "http://localhost:5000/match";

            client.DefaultRequestHeaders.Add("Authorization", $"JWT {jwt.access_token}");

            string serializedMatch = JsonConvert.SerializeObject(match);

            StringContent matchStringContent = new StringContent(serializedMatch, Encoding.UTF8, "application/json");

            HttpResponseMessage matchResponseMessage = await client.PostAsync(url, matchStringContent);

            string matchResponseString = await matchResponseMessage.Content.ReadAsStringAsync();

            MatchResponse matchResponse = JsonConvert.DeserializeObject<MatchResponse>(matchResponseString);

            return matchResponse.id;
        }
    }
}
