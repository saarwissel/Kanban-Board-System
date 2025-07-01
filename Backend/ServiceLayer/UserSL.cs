using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Kanban.Backend.ServiceLayer;
using Kanban.Backend.BusinessLayer;
using System.Text.Json.Serialization;

namespace Kanban.Backend.ServiceLayer
{

    public class UserSL
    {
        public string Email { get; set; }       
        public bool LoggedIn { get; set; }

        public UserSL(string email, bool isLoggedIn)
        {
            Email = email;
            LoggedIn = isLoggedIn;
        }
    }
}


