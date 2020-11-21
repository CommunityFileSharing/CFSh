using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CFSh_backend.API.User
{
    public class Update
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
