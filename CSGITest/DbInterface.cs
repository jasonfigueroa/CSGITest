using Newtonsoft.Json;
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
        public static void SaveToDb(MatchStats matchStats)
        {
            //DateTime TimeStop = DateTime.Now;
            //TimeSpan TimeDiff = TimeStop - matchStats.TimeStart;
            //matchStats.MinutesPlayed = TimeDiff.Minutes;

            string username = "jason";
            string password = "jason";
            UserAuth userAuth = new UserAuth(username, password);

            RunAsync(userAuth, matchStats).GetAwaiter().GetResult();
        }

        static async Task RunAsync(UserAuth userAuth, MatchStats matchStats)
        {
            HttpClient _client = new HttpClient();

            string authUrl = "http://localhost:5000/auth";

            string serializedUserAuth = JsonConvert.SerializeObject(userAuth);
            StringContent authStringContent = new StringContent(serializedUserAuth, Encoding.UTF8, "application/json");

            HttpResponseMessage authResponseMessage = await _client.PostAsync(authUrl, authStringContent);
            string authResponseString = await authResponseMessage.Content.ReadAsStringAsync();

            JWT jwt = JsonConvert.DeserializeObject<JWT>(authResponseString);

            string postMatchStatsUrl = "http://localhost:5000/matchstats";

            if(_client.DefaultRequestHeaders.Authorization != null)
            {
                Console.WriteLine("Authorization Header was not null");
                _client.DefaultRequestHeaders.Authorization = null;
            }

            _client.DefaultRequestHeaders.Add("Authorization", $"JWT {jwt.access_token}");

            string serializedMatchStats = JsonConvert.SerializeObject(matchStats);
            StringContent matchStatsStringContent = new StringContent(serializedMatchStats, Encoding.UTF8, "application/json");

            HttpResponseMessage matchStatsResponseMessage = await _client.PostAsync(postMatchStatsUrl, matchStatsStringContent);

            // temp
            //Console.WriteLine(matchStatsResponseMessage);

            //string matchStatsResponseString = await matchStatsResponseMessage.Content.ReadAsStringAsync();

            //MatchStats returnedMatchStats = JsonConvert.DeserializeObject<MatchStats>(matchStatsResponseString);

            //Console.WriteLine(returnedMatchStats);
        }
    }
}
