using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using log4net;
using Kanban.Backend.DataAccessLayer;

namespace Kanban.Backend.BusinessLayer
{
    /// <summary>
    /// Business logic class representing a user in the system.
    /// </summary>
    public class UserBL
    {
        private string email;
        private string password;
        private UserDAL userDAL;
        private List<BoardBL> boards;
        private bool isLoggedIn;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Email => email;
        public string Password => password;
        public bool IsLoggedIn { get => isLoggedIn; private set => isLoggedIn = value; }
        public List<BoardBL> Boards { get => boards; private set => boards = value; }

        /// <summary>
        /// Constructor for new user registration.
        /// </summary>
        public UserBL(string email, string password)
        {
            this.email = email;
            this.password = password;
            this.boards = new List<BoardBL>();
            this.isLoggedIn = true;
            this.userDAL = new UserDAL(email, password);

            if (!userDAL.Persist())
            {
                log.Info("User persist failed");
                throw new Exception("User persist failed");
            }
        }

        /// <summary>
        /// Constructor for loading user from persisted data.
        /// </summary>
        public UserBL(string email, string password, string nulL)
        {
            this.email = email;
            this.password = password;
            this.boards = new List<BoardBL>();
            this.isLoggedIn = true;
            this.userDAL = new UserDAL(email, password);
        }



        /// <summary>
        /// Attempts to log in the user with the given password.
        /// </summary>
        public void Login(string pass)
        {
            if (!Password.Equals(pass))
            {
                log.Info("Login failed because the password is invalid");
                throw new Exception("Invalid password");
            }
            if (IsLoggedIn)
            {
                log.Info("Login failed because user is already logged in");
                throw new Exception("User is already logged in");
            }
            IsLoggedIn = true;
        }

        /// <summary>
        /// Logs out the user.
        /// </summary>
        public void Logout()
        {
            if (!IsLoggedIn)
            {
                log.Info("Logout failed because user was not logged in");
                throw new Exception("User is already logged out");
            }
            IsLoggedIn = false;
        }

        /// <summary>
        /// Adds a board to the user's list.
        /// </summary>
        public void addBoardToList(BoardBL board)
        {
            if (board != null)
                boards.Add(board);
        }

        /// <summary>
        /// Removes a board from the user's list.
        /// </summary>
        public void removeBoardFromList(BoardBL board)
        {
            if (board != null)
                boards.Remove(board);
        }

        /// <summary>
        /// Retrieves a board by its name.
        /// </summary>
        public BoardBL getBoardByName(string boardName)
        {
            foreach (BoardBL board in boards)
            {
                if (board.GetName().Equals(boardName, StringComparison.OrdinalIgnoreCase))
                {
                    return board;
                }
            }
            throw new Exception("User not a member of this board");
        }

        /// <summary>
        /// Throws exception if a board with the given name already exists.
        /// </summary>
        public void isBoardNameExists(string boardName)
        {
            if (boardName == null)
            {
                throw new Exception("Board name is null");
            }
            foreach (BoardBL board in boards)
            {
                if (board.GetName().Equals(boardName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception("Board name already exists");
                }
            }
        }


        /// <summary>
        /// Throws exception if a board with the given ID already exists.
        /// </summary>
        public void isBoardIdExists(int boardId)
        {
            foreach (BoardBL board in boards)
            {
                if (board.getBoardID() == boardId)
                {
                    throw new Exception("Board ID already exists");
                }
            }
        }
        

        /// <summary>
        /// retun fals if user has board with this name.
        /// </summary>
        public bool hasNameBoard(string boardName)
        {
            foreach (BoardBL board in boards)
            {
                if (board.GetName().Equals(boardName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
