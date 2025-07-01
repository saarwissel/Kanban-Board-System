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
    /// Factory class for initializing and exposing service layer components.
    /// </summary>
    public class ServiceFactory
    {
        public UserService us;
        public BoardService bs;
        public TaskService ts;

        /// <summary>
        /// Initializes all service layer components.
        /// </summary>
        public ServiceFactory()
        {
            AuthenticationFacade authFacade = new AuthenticationFacade(new HashSet<string>());
            UserFacade userFacade = new UserFacade(authFacade);
            BoardFacade boardFacade = new BoardFacade(authFacade, userFacade);
            us = new UserService(userFacade);
            bs = new BoardService(boardFacade);
            ts = new TaskService(boardFacade);
        }

        // ===== BoardService methods (bs) =====

        /// <summary>
        /// Loads all persisted data into the system.
        /// </summary>
        /// <returns>JSON response indicating success or error</returns>
        public string LoadData() => bs.LoadData();

        /// <summary>
        /// Deletes all persisted data from the system.
        /// </summary>
        /// <returns>JSON response indicating success or error</returns>
        public string DeleteData() => bs.DeleteData();

        /// <summary>
        /// Gets the limit of a specific column.
        /// </summary>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal) => bs.GetColumnLimit(email, boardName, columnOrdinal);

        /// <summary>
        /// Gets the name of a specific column.
        /// </summary>
        public string GetColumnName(string email, string boardName, int columnOrdinal) => bs.GetColumnName(email, boardName, columnOrdinal);

        /// <summary>
        /// Adds a task to the specified board.
        /// </summary>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate) => bs.AddTask(email, boardName, title, description, dueDate);

        /// <summary>
        /// Gets the tasks in a specific column.
        /// </summary>
        public string GetColumn(string email, string boardName, int columnOrdinal) => bs.GetColumn(email, boardName, columnOrdinal);

        /// <summary>
        /// Creates a new board.
        /// </summary>
        public string CreateBoard(string email, string name) => bs.CreateBoard(email, name);

        /// <summary>
        /// Deletes a board by name.
        /// </summary>
        public string DeleteBoard(string email, string name) => bs.DeleteBoard(email, name);

        /// <summary>
        /// Joins a user to a board by board ID.
        /// </summary>
        public string JoinBoard(string email, int boardID) => bs.JoinBoard(email, boardID);

        /// <summary>
        /// Removes a user from a board.
        /// </summary>
        public string LeaveBoard(string email, int boardID) => bs.LeaveBoard(email, boardID);

        /// <summary>
        /// Assigns a task to a user.
        /// </summary>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee) => bs.AssignTask(email, boardName, columnOrdinal, taskID, emailAssignee);

        /// <summary>
        /// Gets the name of a board by ID.
        /// </summary>
        public string GetBoardName(int boardId) => bs.GetBoardName(boardId);

        /// <summary>
        /// Transfers board ownership to another user.
        /// </summary>
        public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName) => bs.TransferOwnership(currentOwnerEmail, newOwnerEmail, boardName);

        /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        public string LimitColumn(string email, string boardName, int columnOrdinal, int limit) => bs.LimitColumn(email, boardName, columnOrdinal, limit);




        // ===== UserService methods (us) =====

        /// <summary>
        /// Registers a new user.
        /// </summary>
        public string Register(string email, string password) => us.Register(email, password);

        /// <summary>
        /// Logs a user into the system.
        /// </summary>
        public string Login(string email, string password) => us.Login(email, password);

        /// <summary>
        /// Logs a user out of the system.
        /// </summary>
        public string Logout(string email) => us.Logout(email);

        /// <summary>
        /// Returns all board IDs associated with a user.
        /// </summary>
        public string GetUserBoards(string email) => us.GetUserBoards(email);





        // ===== TaskService methods (ts) =====

        /// <summary>
        /// Updates the due date of a task.
        /// </summary>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate) => ts.UpdateTaskDueDate(email, boardName, columnOrdinal, taskId, dueDate);

        /// <summary>
        /// Updates the title of a task.
        /// </summary>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title) => ts.UpdateTaskTitle(email, boardName, columnOrdinal, taskId, title);

        /// <summary>
        /// Updates the description of a task.
        /// </summary>
        public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description) => ts.UpdateTaskDescription(email, boardName, columnOrdinal, taskId, description);

        /// <summary>
        /// Advances a task to the next column.
        /// </summary>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId) => ts.AdvanceTask(email, boardName, columnOrdinal, taskId);

        /// <summary>
        /// Returns all in-progress tasks for a user.
        /// </summary>
        public string InProgressTasks(string email) => ts.InProgressTasks(email);


        public string GetUserBoardsDetailed(string email) => bs.GetUserBoardsDetailed(email);
        public string GetBoardMembers(string email, string boardName) => bs.GetBoardMembers(email, boardName);
        public string GetBoardTasks(string email, string boardName, int columnOrdinal) => bs.GetBoardTasks(email, boardName, columnOrdinal);



    }
    

    
}
