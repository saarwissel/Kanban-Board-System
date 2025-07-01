using System;
using System.Text.RegularExpressions;
using log4net;
using System.Collections.Generic;
using Kanban.Backend.DataAccessLayer;
using System.Linq;

namespace Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Facade responsible for user-related operations, authentication, and persistence.
    /// </summary>
    public class UserFacade
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal Dictionary<string, UserBL> users;
        private const int MaxEmailLength = 320;
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 20;

        public AuthenticationFacade authFacade;
        private UserController userController;

        public Dictionary<string, UserBL> Users
        {
            get { return users; }
            private set { users = value; }
        }

        /// <summary>
        /// Constructor initializes the facade with empty user map and required controllers.
        /// </summary>
        public UserFacade(AuthenticationFacade authFacade)
        {
            this.users = new Dictionary<string, UserBL>();
            this.userController = new UserController();
            this.authFacade = authFacade;
        }

        /// <summary>
        /// Registers a new user after validating credentials.
        /// </summary>
        public UserBL Register(string email, string password)
        {
            try
            {
                isValidEmail(email);
                if (!isValidPassword(password))
                {
                    log.Info("Registration failed: invalid password.");
                    throw new Exception("Invalid password.");
                }

                var user = new UserBL(email, password);
                authFacade.Login(user.Email);
                users.Add(email, user);

                log.Info($"Registration successful for user: {email}");
                return user;
            }
            catch (Exception ex)
            {
                log.Error($"Registration failed for {email}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Logs in an existing user by validating credentials and session state.
        /// </summary>
        public UserBL Login(string email, string password)
        {
            try
            {
                if (!users.ContainsKey(email))
                    throw new Exception("User does not exist");

                if (authFacade.IsLoggedIn(email))
                    throw new Exception("User is already logged in");

                users[email].Login(password);
                authFacade.Login(email);

                log.Info($"User logged in: {email}");
                return users[email];
            }
            catch (Exception ex)
            {
                log.Error($"Login failed for {email}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Logs out a user and updates system state.
        /// </summary>
        public void Logout(string email)
        {
            try
            {
                if (email == null || !users.ContainsKey(email))
                {
                    log.Warn("Logout attempt with invalid email.");
                    throw new Exception("User does not exist");
                }

                var user = users[email];
                if (!authFacade.IsLoggedIn(email))
                    throw new Exception("User is already logged out");

                user.Logout();
                authFacade.Logout(email);

                log.Info($"User logged out: {email}");
            }
            catch (Exception ex)
            {
                log.Error($"Logout failed for {email}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns a list of board IDs for a specific user.
        /// </summary>
        public List<int> GetUserBoards(string email)
        {
            try
            {
                
                if (!users.ContainsKey(email))
                    throw new Exception("User does not exist");
                if (!authFacade.IsLoggedIn(email))
                {
                    throw new Exception("User not login");
                }
                var boardIdList = new List<int>();
                foreach (var board in users[email].Boards)
                {
                    boardIdList.Add(board.getBoardID());
                }
                return boardIdList.Distinct().ToList();            }
            catch (Exception ex)
            {
                log.Error($"Logout failed for {email}: {ex.Message}");
                throw;
            }
            
        }

        /// <summary>
        /// Loads all persisted user data into memory.
        /// </summary>
        public List<UserDAL> LoadData()
        {
            try
            {
                var listOfUserDAL = userController.AllUsers();
                if (listOfUserDAL == null)
                    throw new Exception("Failed to load user data");

                foreach (var item in listOfUserDAL)
                {
                    if (item == null)
                        throw new Exception("Invalid user record");

                    var user = new UserBL(item.Email, item.Password, "");
                    user.Logout(); 
                    users[item.email] = user;
                    
                }
                log.Info($"Loaded {users.Count} users from database.");
                return listOfUserDAL;
            }
            catch (Exception ex)
            {
                log.Error("Failed to load users: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Deletes all user data from memory and database.
        /// </summary>
        public void DeleteData()
        {
            try
            {
                users.Clear();
                bool success = DeleteUsers();
                if (!success)
                    throw new Exception("Failed to delete user data from DB");

                log.Info("All users deleted successfully.");
            }
            catch (Exception ex)
            {
                log.Error("DeleteData failed: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Helper for deleting all users in the DB.
        /// </summary>
        public bool DeleteUsers()
        {
            return userController.DeleteAll();
        }

        /// <summary>
        /// Validates the format of the given email.
        /// </summary>
        public void isValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || email.Length > MaxEmailLength || !emailValidationInRegex(email) || email.Contains(" "))
            {
                log.Info("Invalid email during registration: " + email);
                throw new Exception("Invalid email.");
            }
            if (users.ContainsKey(email))
            {
                log.Info("Attempt to register duplicate email: " + email);
                throw new Exception("User already exists.");
            }
        }

        private bool emailValidationInRegex(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" +
                            @"([-a-zA-Z0-9!#$%&'*+/=?^_`{|}~](\.?[-a-zA-Z0-9!#$%&'*+/=?^_`{|}~])*){1,64})" +
                            @"@([a-zA-Z0-9]([-a-zA-Z0-9]{0,61}[a-zA-Z0-9])?\.)+" +
                            @"[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        /// <summary>
        /// Validates the password complexity requirements.
        /// </summary>
        private bool isValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < MinPasswordLength || password.Length > MaxPasswordLength)
                return false;

            bool hasLower = false, hasUpper = false, hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsLower(c)) hasLower = true;
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsDigit(c)) hasDigit = true;
            }

            return hasLower && hasUpper && hasDigit;
        }
    }
}
