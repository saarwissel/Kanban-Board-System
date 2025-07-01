using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using log4net;
using Kanban.Backend.BusinessLayer;
using Kanban.Backend.ServiceLayer;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Provides user-related service operations including registration, login, logout, and board access.
    /// </summary>
    public class UserService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private UserFacade userFacade;

        public UserService(UserFacade uf)
        {
            this.userFacade = uf;
        }

        /// <summary>
        /// Registers a new user with a given email and password.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's password.</param>
        /// <returns>A JSON response with no error or with an error message.</returns>
        public string Register(string email, string password)
        {
            
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                log.Error("Email or password is null or empty.");
                return new Response("Email or password cannot be null or empty.").ToJson();
            }
            try
            {
                email = email.Trim().ToLower();
                log.Info($"Attempting to register user: {email}");

                var user = userFacade.Register(email, password);
                log.Info($"Registration successful for user: {email}");

                return new Response(null, null).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"Registration failed for user {email}: {ex.Message}");
                return new Response("Exception: " + ex.Message).ToJson();
            }
        }

        /// <summary>
        /// Logs in a user with the given credentials.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="pass">User's password.</param>
        /// <returns>A JSON response with user's email or error message.</returns>
        public string Login(string email, string pass)
        {
            try
            {
                string result=email;
                if (string.IsNullOrWhiteSpace(email))
                    throw new Exception("Email is null or empty.");

                email = email.Trim().ToLower();
                log.Info($"User login attempt: {email}");

                var user = userFacade.Login(email, pass);
                log.Info($"Login successful: {email}");

                return new Response(null, result).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"Login failed for user {email}: {ex.Message}");
                return new Response("Exception: " +ex.Message).ToJson();
            }
        }

        /// <summary>
        /// Logs out a logged-in user.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>A JSON response indicating success or error.</returns>
        public string Logout(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new Exception("Email is null or empty.");

                email = email.Trim().ToLower();
                log.Info($"Logout attempt for user: {email}");

                userFacade.Logout(email);
                log.Info($"Logout successful: {email}");

                return new Response(null, null).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"Logout failed for user {email}: {ex.Message}");
                return new Response("Exception: " + ex.Message).ToJson();
            }
        }

        /// <summary>
        /// Retrieves all board IDs for a specific user.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>A JSON response with a list of board IDs or error.</returns>
        public string GetUserBoards(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new Exception("Email is null or empty.");

                email = email.Trim().ToLower();
                log.Info($"Retrieving boards for user: {email}");

                List<int> listOfIds = userFacade.GetUserBoards(email);
                log.Info($"Retrieved {listOfIds.Count} boards for user: {email}");

                return new Response(null, listOfIds).ToJson();
            }
            catch (Exception ex)
            {
                log.Error($"Failed to retrieve boards for user {email}: {ex.Message}");
                return new Response("Exception: " + ex.Message).ToJson();
            }
        }
    }
}
