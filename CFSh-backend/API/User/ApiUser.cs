using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CFSh_backend.API.User
{
	public class ApiUser
    {
		    
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}