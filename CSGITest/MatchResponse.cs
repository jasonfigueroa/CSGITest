using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGITest
{
    class MatchResponse
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public double datetime_start { get; set; }
        public int minutes_played { get; set; }
        public string map_name { get; set; }
        public string team { get; set; }
        public string round_win_team { get; set; }
    }
}
