using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGITest
{
    class Player
    {
        public string Name { get; set; }
        public string SteamId { get; set; }

        public Player(string name, string steamId)
        {
            Name = name;
            SteamId = steamId;
        }
    }
}
