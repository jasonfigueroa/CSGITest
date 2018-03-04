using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGITest
{
    class UserAuth
    {
        public string username { get; set; }
        public string password { get; set; }

        public UserAuth(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
