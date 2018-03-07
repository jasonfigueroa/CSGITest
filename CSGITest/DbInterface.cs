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
        public static void SaveToDb(Match match, MatchStats matchStats)
        {
            // TODO need to collect username and password some other way
            string username = "jason";
            string password = "jason";
            UserAuth userAuth = new UserAuth(username, password);

            RunAsync(userAuth, match, matchStats).GetAwaiter().GetResult();
        }

        static async Task RunAsync(UserAuth userAuth, Match match, MatchStats matchStats)
        {
            HttpClient _client = new HttpClient();

            /********/
            /* Auth */
            /********/
            string authUrl = "http://localhost:5000/auth";

            string serializedUserAuth = JsonConvert.SerializeObject(userAuth);
            StringContent authStringContent = new StringContent(serializedUserAuth, Encoding.UTF8, "application/json");

            HttpResponseMessage authResponseMessage = await _client.PostAsync(authUrl, authStringContent);
            string authResponseString = await authResponseMessage.Content.ReadAsStringAsync();

            JWT jwt = JsonConvert.DeserializeObject<JWT>(authResponseString);

            /************************************************/
            /* Post Match - should return match_id from API */
            /************************************************/
            string postMatchUrl = "http://localhost:5000/match";

            if(_client.DefaultRequestHeaders.Authorization != null)
            {
                Console.WriteLine("Authorization Header was not null");
                _client.DefaultRequestHeaders.Authorization = null;
            }

            _client.DefaultRequestHeaders.Add("Authorization", $"JWT {jwt.access_token}");

            string serializedMatch = JsonConvert.SerializeObject(match);

            // for json timestamp
            //string javascriptJson = JsonConvert.SerializeObject(entry, new JavaScriptDateTimeConverter());

            StringContent matchStringContent = new StringContent(serializedMatch, Encoding.UTF8, "application/json");

            HttpResponseMessage matchResponseMessage = await _client.PostAsync(postMatchUrl, matchStringContent);

            // TODO on_post need to extract the match_id from the response
            string matchResponseString = await matchResponseMessage.Content.ReadAsStringAsync();

            MatchResponse matchResponse = JsonConvert.DeserializeObject<MatchResponse>(matchResponseString);

            matchStats.match_id = matchResponse.id;

            /*****************************************************/
            /* Post MatchStats - should return match_id from API */
            /*****************************************************/
            string postMatchStatsUrl = "http://localhost:5000/matchstats";

            string serializedMatchStats = JsonConvert.SerializeObject(matchStats);

            // for json timestamp
            //string javascriptJson = JsonConvert.SerializeObject(entry, new JavaScriptDateTimeConverter());

            StringContent matchStatsStringContent = new StringContent(serializedMatchStats, Encoding.UTF8, "application/json");

            HttpResponseMessage matchStatsResponseMessage = await _client.PostAsync(postMatchStatsUrl, matchStatsStringContent);

            /*************************************/
            /* temporarily keeping the following */
            /*************************************/

            //Console.WriteLine(matchStatsResponseMessage);

            //string matchStatsResponseString = await matchStatsResponseMessage.Content.ReadAsStringAsync();

            //MatchStats returnedMatchStats = JsonConvert.DeserializeObject<MatchStats>(matchStatsResponseString);

            //Console.WriteLine(returnedMatchStats);
        }
    }
}
