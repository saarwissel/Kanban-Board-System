using IntroSE.Kanban.Backend.ServiceLayer;
using Kanban.Backend.BusinessLayer;
using Kanban.Backend.ServiceLayer;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Kanban.Backend.ServiceLayer
{
    public class BoardService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BoardFacade boardFacade;

        public BoardService(BoardFacade boardFacade)
        {
            this.boardFacade = boardFacade;
        }

        /// <summary>
        /// Loads persisted data into the system.
        /// </summary>
        public string LoadData()
        {
            try
            {
                boardFacade.LoadData();
                return new Response(null, null).ToJson();
            }
            catch (Exception ex)
            {
                log.Error("Error in LoadData: " + ex.Message);
                return new Response(ex.Message).ToJson();
            }
        }

        /// <summary>
        /// Deletes all persisted data.
        /// </summary>
        public string DeleteData()
        {
            try
            {
                boardFacade.DeleteData();
                return new Response(null, null).ToJson();
            }
            catch (Exception ex)
            {
                log.Error("Error in DeleteData: " + ex.Message);
                return new Response("Exception: " + ex.Message).ToJson();
            }
        }

        /// <summary>
        /// Creates a new board for the user.
        /// </summary>
        public string CreateBoard(string email, string boardName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName))
                return new Response("Email or board name is null or empty.").ToJson();

            try
            {
                boardFacade.CreateBoard(email.Trim().ToLower(), boardName);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in CreateBoard: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Deletes a board owned by the user.
        /// </summary>
        public string DeleteBoard(string email, string boardName)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName))
                return new Response("Email or board name is null.").ToJson();

            try
            {
                boardFacade.DeleteBoard(email.Trim().ToLower(), boardName);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in DeleteBoard: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Limits the number of tasks in a specific column.
        /// </summary>
        public string LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || columnOrdinal < 0 || columnOrdinal > 2 || limit < -1)
                return new Response("Invalid input.").ToJson();
            try
            {
                boardFacade.LimitColumn(email.Trim().ToLower(), boardName, columnOrdinal, limit);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in LimitColumn: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Gets the task limit of a specific column.
        /// </summary>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || columnOrdinal < 0 || columnOrdinal > 2)
                return new Response("null input.").ToJson();

            try
            {
                int limit = boardFacade.GetColumnLimit(email.Trim().ToLower(), boardName, columnOrdinal);
                return new Response(null, limit).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetColumnLimit: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Gets the name of a specific column.
        /// </summary>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || columnOrdinal < 0 || columnOrdinal > 2)
                return new Response("null input.").ToJson();

            try
            {
                string name = boardFacade.GetColumnName(email.Trim().ToLower(), boardName, columnOrdinal);
                return new Response(null, name).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetColumnName: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Adds a task to the backlog column of a board.
        /// </summary>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                var task = boardFacade.AddTask(email.Trim().ToLower(), boardName, title, description, dueDate);

                return new Response(null, task.taskId).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in AddTask: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Gets all tasks in a specific column.
        /// </summary>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || columnOrdinal < 0 || columnOrdinal > 2)
                return new Response("Invalid input.").ToJson();

            try
            {
                var tasks = boardFacade.GetColumn(email.Trim().ToLower(), boardName, columnOrdinal);
                List<TaskSL> taskSLs = new List<TaskSL>();
                foreach (var t in tasks)
                {
                    taskSLs.Add(new TaskSL(t.getId(), t.getcreationTime(), t.getTitle(), t.getDescription(), t.getDueDate(), t.getBoardId(), t.getEmailAssignee()));
                }
                return new Response(null, taskSLs.ToArray()).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetColumn: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Adds a user as a member to a board.
        /// </summary>
        public string JoinBoard(string email, int boardID)
        {
            if (string.IsNullOrWhiteSpace(email) || boardID < 0)
                return new Response("Email or board ID is invalid.").ToJson();

            try
            {
                boardFacade.JoinBoard(email.Trim().ToLower(), boardID);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in JoinBoard: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Removes a user from a board.
        /// </summary>
        public string LeaveBoard(string email, int boardID)
        {
            if (string.IsNullOrWhiteSpace(email) || boardID < 0)
                return new Response("Email or board ID is invalid.").ToJson();

            try
            {
                boardFacade.LeaveBoard(email.Trim().ToLower(), boardID);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in LeaveBoard: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Gets the name of the board by its ID.
        /// </summary>
        public string GetBoardName(int boardId)
        {
            if (boardId < 0)
                return new Response("Invalid board ID.").ToJson();

            try
            {
                string boardName = boardFacade.getBoardName(boardId);
                return new Response(null, boardName).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetBoardName: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Transfers ownership of a board to another user.
        /// </summary>
        public string TransferOwnership(string currEmail, string newEmail, string boardName)
        {
            if (string.IsNullOrWhiteSpace(currEmail) || string.IsNullOrWhiteSpace(newEmail))
                return new Response("Error: One of the emails is null.").ToJson();

            try
            {
                boardFacade.TransferOwnership(currEmail.Trim().ToLower(), newEmail.Trim().ToLower(), boardName);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in TransferOwnership: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Assigns a task to a specific user.
        /// </summary>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || string.IsNullOrWhiteSpace(emailAssignee) || columnOrdinal < 0 || columnOrdinal > 2 || taskID < 0)
                return new Response("Invalid input.").ToJson();

            try
            {
                boardFacade.AssignTask(email.Trim().ToLower(), boardName, columnOrdinal, taskID, emailAssignee.Trim().ToLower());
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in AssignTask: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }
        
        public string GetUserBoardsDetailed(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new Response("Invalid input.").ToJson();
            try
            {
                var boards = boardFacade.GetUserBoardsDetailed(email.Trim().ToLower());
                return new Response(null, boards).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetUserBoardsDetailed : " + e.Message);
                return new Response($"Error getting boards: {e.Message}").ToJson();
            }
        }
        public string GetBoardsUS(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new Response("Invalid input.").ToJson();
            try
            {
                var boards = boardFacade.GetBoardsUS(email.Trim().ToLower());
                return new Response(null, boards).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetBoardsUS : " + e.Message);
                return new Response($"Error getting boards: {e.Message}").ToJson();
            }
        }

        public string GetBoardMembers(string email, string boardName)
        {
            if (string.IsNullOrWhiteSpace(email)||string.IsNullOrWhiteSpace(boardName))
                return new Response("Invalid input.").ToJson();
            try
            {
                var members = boardFacade.GetBoardMembers(email.Trim().ToLower(), boardName);
                return new Response(null, members).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetBoardMembers : " + e.Message);
                return new Response($"Error getting members: {e.Message}").ToJson();
            }
        }

        public string GetBoardTasks(string email, string boardName, int columnOrdinal)
        {
            if (string.IsNullOrWhiteSpace(email)||string.IsNullOrWhiteSpace(boardName)||columnOrdinal<0||columnOrdinal>2)
                return new Response("Invalid input.").ToJson();
            try
            {
                var tasks = boardFacade.GetBoardTasks(email.Trim().ToLower(), boardName, columnOrdinal);
                List< TaskFd > frontendTask= new List< TaskFd >();
                foreach ( var task in tasks)
                {
                    frontendTask.Add(new TaskFd(task));
                }
                return new Response(null, frontendTask).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in GetBoardTasks : " + e.Message);
                return new Response($"Error getting tasks: {e.Message}").ToJson();
            }
        }

        
    }
}
