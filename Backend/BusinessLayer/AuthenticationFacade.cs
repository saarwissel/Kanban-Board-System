using System;
using System.Text.RegularExpressions;
using log4net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kanban.Backend.BusinessLayer;
using System.Collections.Generic;
using Kanban.Backend.ServiceLayer;

namespace Kanban.Backend.BusinessLayer
{
	public class AuthenticationFacade
	{

		private HashSet<string> loggedInEmails;
		public AuthenticationFacade(HashSet<string> logggedIn)
		{
			this.loggedInEmails = logggedIn;
		}
		public bool IsLoggedIn(string email)
		{
			return loggedInEmails.Contains(email);
		}
		public void Login(string email)
		{
			if (!loggedInEmails.Contains(email))
				loggedInEmails.Add(email);
		}
		public void Logout(string email)
		{
			loggedInEmails.Remove(email);
		}
		public void ClearLoggedInUsers()
		{
			loggedInEmails.Clear();
		}
	
	}
}