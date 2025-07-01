using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;


namespace Kanban.Backend.DataAccessLayer
{
    public class UserDAL
    {
        public string email;
        public string password;
        public UserController uc;

        public UserDAL(string email, string pass)
        {
            this.email = email;
            this.password = pass;
            this.uc = new UserController();
        }

        public string Email
        {
            get { return email; }
            private set { email = value; }
        }

        public string Password
        {
            get { return password; }
            private set { password = value; }
        }

        public bool Persist()
        {
            return uc.Persist(this);
        }


    }
     
}