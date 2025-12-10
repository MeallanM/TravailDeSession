using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravailDeSession
{
    internal class Admin
    {
            public int Id { get; set; }
            public string Username { get; set; } = "";
            // stored hashed password (never reveal)
            public string PasswordHash { get; set; } = "";
    }
}
