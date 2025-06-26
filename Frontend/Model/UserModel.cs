using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.Model
{
    public class UserModel
    {
        public string Email { get; }

        public UserModel(string email)
        {
            Email = email;
        }
    }
}
