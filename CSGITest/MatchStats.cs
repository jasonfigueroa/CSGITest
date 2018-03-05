using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSGITest
{
    internal class MatchStats
    {
        public string match_id { get; set; }
        public int kills { get; set; }
        public int round_kill { get; set; }
        public int round_kill_hs { get; set; }
        public int assists { get; set; }
        public int deaths { get; set; }
        public int mvps { get; set; }
        public int score { get; set; }
    }
}